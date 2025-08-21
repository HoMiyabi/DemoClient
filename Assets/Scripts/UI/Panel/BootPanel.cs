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

        public GameObject DialogPanelPrefab;
        private PatchController patchCtrl;

        private void Start()
        {
            StatusText.text = string.Empty;
            Boot().Forget();
        }

        private void SetStatus(string text)
        {
            Debug.Log(text);
            StatusText.text = text;
        }

        private async UniTaskVoid Boot()
        {
            SetStatus("连接服务器...");
            NetMgr.Instance.Init();
            NetMgr.Instance.Connect();

            SetStatus("加载资源...");
            YooAssets.Initialize();

            patchCtrl = new PatchController("DefaultPackage")
            {
                OnInitPackageFailed = OnInitPackageFailed,
                OnRequestPackageVersionFailed = OnRequestPackageVersionFailed,
                OnUpdatePackageManifestFailed = OnUpdatePackageManifestFailed,
                OnFoundUpdateFiles = OnFoundUpdateFiles,
                OnWebFileDownloadFailed = OnWebFileDownloadFailed,
                OnDownloadUpdate = OnDownloadUpdate
            };
            await patchCtrl.PatchAsync();

            var package = YooAssets.GetPackage("DefaultPackage");
            YooAssets.SetDefaultPackage(package);

            SetStatus("初始化脚本...");
            LuaMgr.Instance.Init();

            SetStatus("加载配置...");
            ConfigMgr.LoadTables();

            SetStatus("点击登录");
            BgBtn.onClick.AddListener(() =>
            {
                var login = UIMgr.Instance.PushPanel<LoginDialogPanel>();
                login.onClosed = UniTask.Action(async () =>
                {
                    if (!login.LoginSuccess) return;

                    BgBtn.onClick.RemoveAllListeners();

                    Debug.Log("获取数据...");
                    StatusText.text = "获取数据...";
                    await PlayerService.FetchData();

                    Debug.Log("初始化设置...");
                    StatusText.text = "初始化设置...";
                    SettingsMgr.Init(PlayerService.Player.Uid);

                    Debug.Log("点击进入游戏");
                    StatusText.text = "点击进入游戏";

                    BgBtn.onClick.AddListener(() => { LoadSceneMgr.Instance.LoadScene("CombatScene"); });
                });
            });
        }

        private void OnInitPackageFailed(Action retry)
        {
            var panel = UIMgr.Instance.PushPanel<DialogPanel>(DialogPanelPrefab);
            panel.Title = "提示";
            panel.Content = "初始化资源失败";
            panel.OkText = "重试";
            panel.HasCloseBtn = false;
            panel.OkBtnOnClick.AddListener(() =>
            {
                UIMgr.Instance.PopPanel(panel);
                retry();
            });
        }

        private void OnRequestPackageVersionFailed(Action retry)
        {
            var panel = UIMgr.Instance.PushPanel<DialogPanel>(DialogPanelPrefab);
            panel.Title = "提示";
            panel.Content = "请求资源版本失败";
            panel.OkText = "重试";
            panel.HasCloseBtn = false;
            panel.OkBtnOnClick.AddListener(() =>
            {
                UIMgr.Instance.PopPanel(panel);
                retry();
            });
        }

        private void OnUpdatePackageManifestFailed(Action retry)
        {
            var panel = UIMgr.Instance.PushPanel<DialogPanel>(DialogPanelPrefab);
            panel.Title = "提示";
            panel.Content = "更新资源清单失败";
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
                retry();
            });
        }

        private void OnDownloadUpdate(DownloadUpdateData data)
        {
            UIProgressBar.Progress = data.Progress;
        }
    }
}