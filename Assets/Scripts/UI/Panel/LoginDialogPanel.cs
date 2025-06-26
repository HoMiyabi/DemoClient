using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class LoginDialogPanel : BasePanel
    {
        private TMP_InputField PasswordInput;
        private TMP_InputField UsernameInput;
        private Button LoginBtn;
        private Button RegisterBtn;
        private Button UICloseBtn;

        public bool loginSuccess = false;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            PasswordInput = c.dict["PasswordInput"] as TMP_InputField;
            UsernameInput = c.dict["UsernameInput"] as TMP_InputField;
            LoginBtn = c.dict["LoginBtn"] as Button;
            RegisterBtn = c.dict["RegisterBtn"] as Button;
            UICloseBtn = c.dict["UICloseBtn"] as Button;
        }

        private void Awake()
        {
            InitUI();

            UICloseBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
            LoginBtn.onClick.AddListener(() => btnLogin_onClick().Forget());
            RegisterBtn.onClick.AddListener(() => btnRegister_onClick().Forget());
        }

        private async UniTaskVoid btnLogin_onClick()
        {
            var rsp = await NetFn.ReqLogin(new ReqLogin
            {
                Username = UsernameInput.text,
                Password = PasswordInput.text
            });

            if (rsp.Code == 0)
            {
                loginSuccess = true;
                UIMgr.Instance.PopPanel(this);
            }
            else
            {
                var panel = UIMgr.Instance.PushPanel<AlterDialogPanel>();
                panel.Title = "提示";
                panel.Content = rsp.Msg;
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
            panel.Content = rsp.Msg;
            panel.OkClickedEvent.AddListener(() => UIMgr.Instance.PopPanel(panel));
        }
    }
}