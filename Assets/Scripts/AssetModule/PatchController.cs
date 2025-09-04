using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace Manager
{
    public class PatchController
    {
        public string PackageName { get; private set; }
        public string PackageVersion { get; private set; }
        public ResourceDownloaderOperation Downloader { get; private set; }

        public delegate void InitPackageFailedDel(Action retry);
        public delegate void RequestPackageVersionFailedDel(Action retry);
        public delegate void UpdatePackageManifestFailedDel(Action retry);
        public delegate void FoundUpdateFilesDel(int totalCount, long totalBytes, Action startDownload);
        public delegate void WebFileDownloadFailedDel(DownloadErrorData data, Action retry);

        public InitPackageFailedDel OnInitPackageFailed;
        public RequestPackageVersionFailedDel OnRequestPackageVersionFailed;
        public UpdatePackageManifestFailedDel OnUpdatePackageManifestFailed;
        public FoundUpdateFilesDel OnFoundUpdateFiles;
        public WebFileDownloadFailedDel OnWebFileDownloadFailed;
        public DownloaderOperation.DownloadUpdate OnDownloadUpdate;
        public Action OnDownloadSuccess;

        private DownloadErrorData downloadErrorData;

        private AsyncOperationBase op;

        public PatchController(string packageName)
        {
            PackageName = packageName;
        }

        public async UniTask PatchAsync()
        {
            // 1. 初始化Package
            // 2. 请求最新Package版本
            // 3. 更新Package清单
            // 4. 创建下载器
            // 5. 下载资源包文件
            // 6. 清理缓存

            // 可能的执行路径:
            // 1 -> 2 -> 3 -> 4 (无需更新)
            // 1 -> 2 -> 3 -> 4 -> 5 -> 6 (有更新)
            // 1 -> 2 -> 3 -> 4 -> 5 (-> 4 -> 5...) -> 6 (有更新且下载失败)
            // 1, 2, 3失败原地反复尝试
            // 没有用户可选的多分支

            // 1. 初始化Package
            while (true)
            {
                await InitPackage().ToUniTask();
                if (op.Status == EOperationStatus.Succeed)
                {
                    Log("Package初始化成功");
                    break;
                }
                // 失败
                LogWarning($"Package初始化失败：{op.Error}");

                // 请求重试
                var utcs = new UniTaskCompletionSource();
                OnInitPackageFailed?.Invoke(() => utcs.TrySetResult());
                await utcs.Task;
            }

            // 2. 请求最新Package版本
            while (true)
            {
                await RequestPackageVersion().ToUniTask();
                if (op.Status == EOperationStatus.Succeed)
                {
                    // 获取Package版本
                    Log($"请求最新package版本成功: {PackageVersion}");
                    break;
                }
                // 失败
                LogWarning($"请求最新package版本失败: {op.Error}");

                // 请求重试
                var utcs = new UniTaskCompletionSource();
                OnRequestPackageVersionFailed?.Invoke(() => utcs.TrySetResult());
                await utcs.Task;
            }

            // 3. 更新Package清单
            while (true)
            {
                await UpdatePackageManifest().ToUniTask();
                if (op.Status == EOperationStatus.Succeed)
                {
                    // 更新成功
                    Log("更新Package清单成功");
                    break;
                }
                // 更新失败
                LogWarning(op.Error);

                // 请求重试
                var utcs = new UniTaskCompletionSource();
                OnUpdatePackageManifestFailed?.Invoke(() => utcs.TrySetResult());
                await utcs.Task;
            }

            // 4和5
            while (true)
            {
                // 4. 创建下载器
                CreateDownloader();
                if (Downloader.TotalDownloadCount == 0)
                {
                    Log("没有文件需要下载");
                    return;
                }

                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                int totalDownloadCount = Downloader.TotalDownloadCount;
                long totalDownloadBytes = Downloader.TotalDownloadBytes;

                // 请求开始下载
                var utcsStartDownload = new UniTaskCompletionSource();
                OnFoundUpdateFiles?.Invoke(totalDownloadCount, totalDownloadBytes,
                    () => utcsStartDownload.TrySetResult());
                await utcsStartDownload.Task;

                // 5. 下载资源包文件
                await DownloadPackageFiles().ToUniTask();
                if (op.Status == EOperationStatus.Succeed)
                {
                    Log("资源文件下载完毕");
                    OnDownloadSuccess?.Invoke();
                    break;
                }
                // 下载失败
                LogWarning(op.Error);

                // 请求重试
                var utcsRetry = new UniTaskCompletionSource();
                OnWebFileDownloadFailed?.Invoke(downloadErrorData, () => utcsRetry.TrySetResult());
                await utcsRetry.Task;
            }

            // 6. 清理缓存
            await ClearCacheBundle().ToUniTask();
            if (op.Status == EOperationStatus.Succeed)
            {
                Log("清理缓存文件完毕");
            }
            else
            {
                LogWarning(op.Error);
            }
        }

        private static void Log(object message)
        {
            Debug.Log("[Asset] " + message);
        }

        private static void LogWarning(object message)
        {
            Debug.LogWarning("[Asset] " + message);
        }

        private IEnumerator InitPackage()
        {
            // Step = EStep.InitPackage;
            Log("初始化资源包");

            // 创建资源包裹类
            var package = YooAssets.TryGetPackage(PackageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(PackageName);
            }

            // 初始化参数
            InitializeParameters initParams = null;
            switch (AssetMgr.Instance.playMode)
            {
                // 编辑器模拟模式
                case EPlayMode.EditorSimulateMode:
                {
                    Log("编辑器模拟模式");
                    var buildResult = EditorSimulateModeHelper.SimulateBuild(PackageName);
                    string packageRoot = buildResult.PackageRootDirectory;
                    initParams = new EditorSimulateModeParameters
                    {
                        EditorFileSystemParameters = FileSystemParameters
                            .CreateDefaultEditorFileSystemParameters(packageRoot)
                    };
                    break;
                }
                // 单机运行模式
                case EPlayMode.OfflinePlayMode:
                {
                    Log("单机运行模式");
                    initParams = new OfflinePlayModeParameters
                    {
                        BuildinFileSystemParameters = FileSystemParameters
                            .CreateDefaultBuildinFileSystemParameters()
                    };
                    break;
                }
                // 联机运行模式
                case EPlayMode.HostPlayMode:
                {
                    Log("联机运行模式");

                    string defaultHostServer = GetHostServerURL();
                    string fallbackHostServer = GetHostServerURL();
                    var remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);

                    initParams = new HostPlayModeParameters()
                    {
                        BuildinFileSystemParameters = FileSystemParameters
                            .CreateDefaultBuildinFileSystemParameters(),
                        CacheFileSystemParameters = FileSystemParameters
                            .CreateDefaultCacheFileSystemParameters(remoteServices)
                    };
                    break;
                }
            }

            var operation = package.InitializeAsync(initParams);
            yield return operation;
            op = operation;
        }

        private IEnumerator RequestPackageVersion()
        {
            // Step = EStep.RequestPackageVersion;
            Log("请求资源版本");

            var package = YooAssets.GetPackage(PackageName);
            var operation = package.RequestPackageVersionAsync();
            yield return operation;
            PackageVersion = operation.PackageVersion;
            op = operation;
        }

        private IEnumerator UpdatePackageManifest()
        {
            // Step = EStep.UpdatePackageManifest;
            Log("更新资源清单");

            var package = YooAssets.GetPackage(PackageName);
            var operation = package.UpdatePackageManifestAsync(PackageVersion);
            yield return operation;
            op = operation;
        }

        private void CreateDownloader()
        {
            // Step = EStep.CreateDownloader;
            Log("创建资源下载器");

            var package = YooAssets.GetPackage(PackageName);
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            Downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        }

        private IEnumerator DownloadPackageFiles()
        {
            // Step = EStep.DownloadPackageFiles;
            Log("下载资源");

            Downloader.DownloadErrorCallback = OnDownloadError;
            Downloader.DownloadUpdateCallback = OnDownloadUpdate;
            Downloader.BeginDownload();
            yield return Downloader;
            op = Downloader;
        }

        private void OnDownloadError(DownloadErrorData data)
        {
            downloadErrorData = data;
        }

        private IEnumerator ClearCacheBundle()
        {
            var package = YooAssets.GetPackage(PackageName);
            var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
            yield return operation;
            op = operation;
        }

        /// <summary>
        /// 获取资源服务器地址
        /// </summary>
        private string GetHostServerURL()
        {
            //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
            string hostServerIP = AssetMgr.Instance.hostServer;
            string appVersion = "v1.0";

#if UNITY_EDITOR
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                return $"{hostServerIP}/CDN/Android/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                return $"{hostServerIP}/CDN/IPhone/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
                return $"{hostServerIP}/CDN/WebGL/{appVersion}";
            else
                return $"{hostServerIP}/CDN/PC/{appVersion}";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostServerIP}/CDN/Android/{appVersion}";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostServerIP}/CDN/IPhone/{appVersion}";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{hostServerIP}/CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
        }

        /// <summary>
        /// 远端资源地址查询服务类
        /// </summary>
        private class RemoteServices : IRemoteServices
        {
            private readonly string _defaultHostServer;
            private readonly string _fallbackHostServer;

            public RemoteServices(string defaultHostServer, string fallbackHostServer)
            {
                _defaultHostServer = defaultHostServer;
                _fallbackHostServer = fallbackHostServer;
            }
            string IRemoteServices.GetRemoteMainURL(string fileName)
            {
                return $"{_defaultHostServer}/{fileName}";
            }
            string IRemoteServices.GetRemoteFallbackURL(string fileName)
            {
                return $"{_fallbackHostServer}/{fileName}";
            }
        }
    }
}