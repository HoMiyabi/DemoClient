using System.Collections;
using Kirara;
using UnityEngine;
using YooAsset;

namespace Manager
{
    public class AssetMgr : UnitySingleton<AssetMgr>
    {
        public EPlayMode playMode;
        public string packageName = "DefaultPackage";

        public ResourcePackage package;

        private string packageVersion;

        private IEnumerator InitPackageEditorSimulateMode()
        {
            yield break;
        }

        public IEnumerator InitPackage()
        {
            InitializeParameters initParams = null;
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                Debug.Log("开始编辑器模拟打包");
                var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                var packageRoot = buildResult.PackageRootDirectory;
                var editorFileSystemParams = FileSystemParameters
                    .CreateDefaultEditorFileSystemParameters(packageRoot);
                initParams = new EditorSimulateModeParameters
                {
                    EditorFileSystemParameters = editorFileSystemParams
                };
            }
            else if (playMode == EPlayMode.OfflinePlayMode)
            {
                initParams = new OfflinePlayModeParameters
                {
                    BuildinFileSystemParameters = FileSystemParameters
                        .CreateDefaultBuildinFileSystemParameters()
                };
            }

            var initOp = package.InitializeAsync(initParams);
            yield return initOp;

            if (initOp.Status == EOperationStatus.Succeed)
            {
                Debug.Log("Package初始化成功");
            }
            else
            {
                Debug.LogError($"Package初始化失败：{initOp.Error}");
            }
        }

        public IEnumerator RequestPackageVersion()
        {
            var operation = package.RequestPackageVersionAsync();
            yield return operation;

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                packageVersion = operation.PackageVersion;
                Debug.Log($"Request package version: {packageVersion}");
            }
            else
            {
                //更新失败
                Debug.LogError(operation.Error);
            }
        }

        public IEnumerator UpdatePackageManifest()
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            var operation = package.UpdatePackageManifestAsync(packageVersion);
            yield return operation;

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                Debug.Log("更新Package manifest成功");
            }
            else
            {
                //更新失败
                Debug.LogError(operation.Error);
            }
        }

        public IEnumerator InitAll()
        {
            YooAssets.Initialize();
            package = YooAssets.CreatePackage(packageName);
            YooAssets.SetDefaultPackage(package);
            yield return InitPackage();
            yield return RequestPackageVersion();
            yield return UpdatePackageManifest();
        }

        public GameObject InstantiateGO(string location)
        {
            var handle = package.LoadAssetSync<GameObject>(location);
            var go = handle.InstantiateSync();
            handle.Release();
            return go;
        }

        public GameObject InstantiateGO(string location, Transform parent)
        {
            var handle = package.LoadAssetSync<GameObject>(location);
            var go = handle.InstantiateSync(parent);
            handle.Release();
            return go;
        }
    }
}