using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Kirara.Manager;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class BootPanel : BasePanel
    {
        private Button BgBtn;
        private TextMeshProUGUI StatusText;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            BgBtn = c.Q<Button>("BgBtn");
            StatusText = c.Q<TextMeshProUGUI>("StatusText");
        }

        private void Awake()
        {
            InitUI();

            StatusText.text = string.Empty;
        }

        private void Start()
        {
            Boot().Forget();
        }

        private async UniTaskVoid Boot()
        {
            Debug.Log("连接服务器...");
            StatusText.text = "连接服务器...";
            NetMgr.Instance.Init();
            NetMgr.Instance.Connect();

            Debug.Log("初始化资源...");
            StatusText.text = "初始化资源...";
            await AssetMgr.Instance.InitAll().ToUniTask();

            Debug.Log("加载配置...");
            StatusText.text = "加载配置...";
            ConfigMgr.LoadTables();

            Debug.Log("点击登录");
            StatusText.text = "点击登录";
            BgBtn.onClick.AddListener(() =>
            {
                var login = UIMgr.Instance.PushPanel<LoginDialogPanel>();
                login.onClosed = UniTask.Action(async () =>
                {
                    if (!login.loginSuccess) return;

                    BgBtn.onClick.RemoveAllListeners();

                    Debug.Log("获取数据...");
                    StatusText.text = "获取数据...";
                    await PlayerService.FetchData();

                    Debug.Log("初始化设置...");
                    StatusText.text = "初始化设置...";
                    SettingsMgr.Init(PlayerService.player.playerInfo.UId);

                    Debug.Log("点击进入游戏");
                    StatusText.text = "点击进入游戏";

                    BgBtn.onClick.AddListener(() => { LoadSceneMgr.Instance.LoadScene("CombatScene"); });
                });
            });
        }

    }
}