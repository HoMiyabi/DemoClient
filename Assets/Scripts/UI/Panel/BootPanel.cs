using System;
using Cysharp.Threading.Tasks;
using Kirara.Manager;
using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara.UI.Panel
{
    public class BootPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button   BgBtn;
        private TMPro.TextMeshProUGUI   StatusText;
        private Kirara.UI.UIProgressBar UIProgressBar;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b         = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            BgBtn         = b.Q<UnityEngine.UI.Button>(0, "BgBtn");
            StatusText    = b.Q<TMPro.TextMeshProUGUI>(1, "StatusText");
            UIProgressBar = b.Q<Kirara.UI.UIProgressBar>(2, "UIProgressBar");
        }
        #endregion

        public string mainSceneName;
        public GameObject DialogPanelPrefab;
        private PatchController patchCtrl;

        private void Start()
        {
            StatusText.text = string.Empty;
            Boot().Forget();
        }

        private void SetStatusText(string text)
        {
            Debug.Log("[BootPanel] " + text);
            StatusText.text = text;
        }

        private async UniTaskVoid Boot()
        {
            SetStatusText("加载资源...");
            await UniTask.WaitForSeconds(0.3f);
            YooAssets.Initialize();

            patchCtrl = new PatchController("DefaultPackage")
            {
                OnInitPackageFailed = OnInitPackageFailed,
                OnRequestPackageVersionFailed = OnRequestPackageVersionFailed,
                OnUpdatePackageManifestFailed = OnUpdatePackageManifestFailed,
                OnFoundUpdateFiles = OnFoundUpdateFiles,
                OnWebFileDownloadFailed = OnWebFileDownloadFailed,
                OnDownloadUpdate = OnDownloadUpdate,
                OnDownloadSuccess = OnDownloadSuccess
            };
            await patchCtrl.PatchAsync();

            var package = YooAssets.GetPackage("DefaultPackage");
            YooAssets.SetDefaultPackage(package);


            SetStatusText("连接网络...");
            await UniTask.WaitForSeconds(0.3f);
            NetMgr.Instance.Init();
            await NetMgr.Instance.ConnectAsync(OnConnectionFailed);

            SetStatusText("初始化脚本...");
            await UniTask.WaitForSeconds(0.3f);
            LuaMgr.Instance.Init();


            SetStatusText("加载配置...");
            await UniTask.WaitForSeconds(0.3f);
            ConfigMgr.LoadTables();


            SetStatusText("点击登录");
            var loginUtcs = new UniTaskCompletionSource();
            BgBtn.onClick.AddListener(() =>
            {
                var loginPanel = UIMgr.Instance.PushPanel<LoginDialogPanel>();
                loginPanel.OnLoginSuccess = () =>
                {
                    loginUtcs.TrySetResult();
                };
            });

            await loginUtcs.Task;
            BgBtn.onClick.RemoveAllListeners();

            SetStatusText("获取数据...");
            await UniTask.WaitForSeconds(0.3f);
            await PlayerService.FetchData();

            SetStatusText("初始化设置...");
            await UniTask.WaitForSeconds(0.3f);
            SettingsMgr.Init(PlayerService.Player.Uid);

            SetStatusText("点击进入");
            BgBtn.onClick.AddListener(() =>
            {
                LoadSceneMgr.Instance.LoadScene(mainSceneName);
            });
        }

        private void OnInitPackageFailed(string message, Action retry)
        {
            var panel = UIMgr.Instance.PushPanel<DialogPanel>(DialogPanelPrefab);
            panel.Title = "提示";
            panel.Content = "初始化资源失败: " + message;
            panel.OkText = "重试";
            panel.HasCloseBtn = false;
            panel.OkBtnOnClick.AddListener(() =>
            {
                UIMgr.Instance.PopPanel(panel);
                retry();
            });
        }

        private void OnRequestPackageVersionFailed(string message, Action retry)
        {
            var panel = UIMgr.Instance.PushPanel<DialogPanel>(DialogPanelPrefab);
            panel.Title = "提示";
            panel.Content = "请求资源版本失败: " + message;
            panel.OkText = "重试";
            panel.HasCloseBtn = false;
            panel.OkBtnOnClick.AddListener(() =>
            {
                UIMgr.Instance.PopPanel(panel);
                retry();
            });
        }

        private void OnUpdatePackageManifestFailed(string message, Action retry)
        {
            var panel = UIMgr.Instance.PushPanel<DialogPanel>(DialogPanelPrefab);
            panel.Title = "提示";
            panel.Content = "更新资源清单失败: " + message;
            panel.OkText = "重试";
            panel.HasCloseBtn = false;
            panel.OkBtnOnClick.AddListener(() =>
            {
                UIMgr.Instance.PopPanel(panel);
                retry();
            });
        }

        private void OnFoundUpdateFiles(int totalCount, long totalBytes, Action startDownload)
        {
            var panel = UIMgr.Instance.PushPanel<DialogPanel>(DialogPanelPrefab);
            panel.Title = "发现更新";
            panel.Content = $"文件：{totalCount}个，大小：{totalBytes * AssetMgr.BToMB:F2}MB";
            panel.OkText = "更新";
            panel.HasCloseBtn = false;
            panel.OkBtnOnClick.AddListener(() =>
            {
                UIMgr.Instance.PopPanel(panel);
                ShowProgressBar(0f);
                startDownload();
            });
        }

        private void OnWebFileDownloadFailed(DownloadErrorData data, Action retry)
        {
            var panel = UIMgr.Instance.PushPanel<DialogPanel>(DialogPanelPrefab);
            panel.Title = "下载失败";
            panel.Content = $"包裹名: {data.PackageName}\n文件名: {data.FileName}\n错误信息: {data.ErrorInfo}";
            panel.OkText = "重试";
            panel.HasCloseBtn = false;
            panel.OkBtnOnClick.AddListener(() =>
            {
                UIMgr.Instance.PopPanel(panel);
                ShowProgressBar(0f);
                retry();
            });
        }

        private void OnDownloadSuccess()
        {
            HideProgressBar();
        }

        private void OnDownloadUpdate(DownloadUpdateData data)
        {
            UIProgressBar.Progress = data.Progress;
        }

        private void OnConnectionFailed(string message, Action retry)
        {
            Debug.LogWarning("连接失败: " + message);

            var panel = UIMgr.Instance.PushPanel<DialogPanel>(DialogPanelPrefab);
            panel.Title = "连接失败";
            panel.Content = message;
            panel.OkText = "重试";
            panel.HasCloseBtn = false;
            panel.OkBtnOnClick.AddListener(() =>
            {
                UIMgr.Instance.PopPanel(panel);
                retry();
            });
        }

        private void ShowProgressBar(float progress)
        {
            UIProgressBar.gameObject.SetActive(true);
            UIProgressBar.Progress = progress;
        }

        private void HideProgressBar()
        {
            UIProgressBar.gameObject.SetActive(false);
        }
    }
}