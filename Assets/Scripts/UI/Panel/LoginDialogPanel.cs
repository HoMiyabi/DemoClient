using System;
using Cysharp.Threading.Tasks;
using Kirara.Network;

namespace Kirara.UI.Panel
{
    public class LoginDialogPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button UICloseBtn;
        private TMPro.TMP_InputField  UsernameInput;
        private TMPro.TMP_InputField  PasswordInput;
        private UnityEngine.UI.Button LoginBtn;
        private UnityEngine.UI.Button RegisterBtn;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b         = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UICloseBtn    = b.Q<UnityEngine.UI.Button>(0, "UICloseBtn");
            UsernameInput = b.Q<TMPro.TMP_InputField>(1, "UsernameInput");
            PasswordInput = b.Q<TMPro.TMP_InputField>(2, "PasswordInput");
            LoginBtn      = b.Q<UnityEngine.UI.Button>(3, "LoginBtn");
            RegisterBtn   = b.Q<UnityEngine.UI.Button>(4, "RegisterBtn");
        }
        #endregion

        public bool LoginSuccess { get; private set; }

        private void Start()
        {
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
                var panel = UIMgr.Instance.PushPanel<DialogPanel>();
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

            var panel = UIMgr.Instance.PushPanel<DialogPanel>();
            panel.Title = "提示";
            panel.Content = rsp.Result.Msg;
            panel.OkClickedEvent.AddListener(() => UIMgr.Instance.PopPanel(panel));
        }
    }
}