using UnityEngine.UI;
using XLua;

namespace Kirara.UI.Panel
{
    [LuaCallCSharp]
    public class DialogPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Image      BoxBgImg;
        private TMPro.TextMeshProUGUI     TitleText;
        private TMPro.TextMeshProUGUI     ContentText;
        private UnityEngine.UI.Button     OkBtn;
        private UnityEngine.UI.Button     CloseBtn;
        private UnityEngine.RectTransform Box;
        private UnityEngine.UI.Image      BgImg;
        private UnityEngine.CanvasGroup   CanvasGroup;
        private TMPro.TextMeshProUGUI     OkBtnText;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b       = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            BoxBgImg    = b.Q<UnityEngine.UI.Image>(0, "BoxBgImg");
            TitleText   = b.Q<TMPro.TextMeshProUGUI>(1, "TitleText");
            ContentText = b.Q<TMPro.TextMeshProUGUI>(2, "ContentText");
            OkBtn       = b.Q<UnityEngine.UI.Button>(3, "OkBtn");
            CloseBtn    = b.Q<UnityEngine.UI.Button>(4, "CloseBtn");
            Box         = b.Q<UnityEngine.RectTransform>(5, "Box");
            BgImg       = b.Q<UnityEngine.UI.Image>(6, "BgImg");
            CanvasGroup = b.Q<UnityEngine.CanvasGroup>(7, "CanvasGroup");
            OkBtnText   = b.Q<TMPro.TextMeshProUGUI>(8, "OkBtnText");
        }
        #endregion

        private void Start()
        {
            CloseBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }

        public override void PlayEnter()
        {
            DialogPlayEnter(CanvasGroup, Box);
        }

        public override void PlayExit()
        {
            DialogPlayExit(CanvasGroup, Box);
        }

        public string Title
        {
            get => TitleText.text;
            set => TitleText.text = value;
        }

        public string Content
        {
            get => ContentText.text;
            set => ContentText.text = value;
        }

        public Button.ButtonClickedEvent OkBtnOnClick => OkBtn.onClick;

        public bool HasCloseBtn
        {
            get => CloseBtn.gameObject.activeSelf;
            set => CloseBtn.gameObject.SetActive(value);
        }

        public string OkText
        {
            get => OkBtnText.text;
            set => OkBtnText.text = value;
        }
    }
}