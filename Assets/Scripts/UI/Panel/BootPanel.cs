using Cysharp.Threading.Tasks;
using Kirara.Manager;
using Manager;
using UnityEngine;

namespace Kirara.UI.Panel
{
    public class BootPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button BgBtn;
        private TMPro.TextMeshProUGUI StatusText;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b      = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            BgBtn      = b.Q<UnityEngine.UI.Button>(0, "BgBtn");
            StatusText = b.Q<TMPro.TextMeshProUGUI>(1, "StatusText");
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            StatusText.text = string.Empty;
        }

        private void Start()
        {
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

            SetStatus("初始化资源...");
            await AssetMgr.Instance.InitAll().ToUniTask();

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

    }
}