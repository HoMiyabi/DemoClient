using DG.Tweening;
using UnityEngine;
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
        private UnityEngine.RectTransform BoxTra;
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
            BoxTra      = b.Q<UnityEngine.RectTransform>(5, "BoxTra");
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
            CanvasGroup.alpha = 0.9f;
            var t1 = CanvasGroup.DOFade(1f, 0.05f);

            BoxTra.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            var t2 = BoxTra.DOScale(1f, 0.05f);

            t2.onComplete = () => onPlayEnterFinished?.Invoke();
        }

        public override void PlayExit()
        {
            CanvasGroup.DOFade(0.9f, 0.05f);

            var t = BoxTra.DOScale(0.9f, 0.05f);

            t.onComplete = () => onPlayExitFinished?.Invoke();
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