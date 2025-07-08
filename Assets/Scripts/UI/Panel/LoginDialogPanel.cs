using Cysharp.Threading.Tasks;
using Kirara.Network;
using TMPro;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class LoginDialogPanel : BasePanel
    {
        #region View
        private Button UICloseBtn;
        private TMP_InputField UsernameInput;
        private TMP_InputField PasswordInput;
        private Button LoginBtn;
        private Button RegisterBtn;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            UICloseBtn = c.Q<Button>(0, "UICloseBtn");
            UsernameInput = c.Q<TMP_InputField>(1, "UsernameInput");
            PasswordInput = c.Q<TMP_InputField>(2, "PasswordInput");
            LoginBtn = c.Q<Button>(3, "LoginBtn");
            RegisterBtn = c.Q<Button>(4, "RegisterBtn");
        }
        #endregion

        public bool LoginSuccess { get; private set; }

        private void Awake()
        {
            InitUI();

            UICloseBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
            LoginBtn.onClick.AddListener(() => btnLogin_onClick().Forget());
            RegisterBtn.onClick.AddListener(() => btnRegister_onClick().Forget());
        }

        private async UniTaskVoid btnLogin_onClick()
        {
            try
            {
                var rsp = await NetFn.ReqLogin(new ReqLogin
                {
                    Username = UsernameInput.text,
                    Password = PasswordInput.text
                });
                LoginSuccess = true;
                UIMgr.Instance.PopPanel(this);
            }
            catch (ResultException e)
            {
                var rsp = (RspLogin)e.Msg;
                var panel = UIMgr.Instance.PushPanel<AlterDialogPanel>();
                panel.Title = "提示";
                panel.Content = rsp.Result.Msg;
                panel.OkClickedEvent.AddListener(() => UIMgr.Instance.PopPanel(panel));
            }
        }

        private async UniTaskVoid btnRegister_onClick()
        {
            var rsp = await NetFn.ReqRegister(new ReqRegister
            {
                Username = UsernameInput.text,
                Password = PasswordInput.text
            });

            var panel = UIMgr.Instance.PushPanel<AlterDialogPanel>();
            panel.Title = "提示";
            panel.Content = rsp.Result.Msg;
            panel.OkClickedEvent.AddListener(() => UIMgr.Instance.PopPanel(panel));
        }
    }
}