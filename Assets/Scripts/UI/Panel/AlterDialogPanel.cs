using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class AlterDialogPanel : BasePanel
    {
        #region View
        private Image BoxBgImg;
        private TextMeshProUGUI TitleText;
        private TextMeshProUGUI ContentText;
        private Button OkBtn;
        private Button CloseBtn;
        private RectTransform BoxTra;
        private Image BgImg;
        private CanvasGroup CanvasGroup;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            BoxBgImg = c.Q<Image>(0, "BoxBgImg");
            TitleText = c.Q<TextMeshProUGUI>(1, "TitleText");
            ContentText = c.Q<TextMeshProUGUI>(2, "ContentText");
            OkBtn = c.Q<Button>(3, "OkBtn");
            CloseBtn = c.Q<Button>(4, "CloseBtn");
            BoxTra = c.Q<RectTransform>(5, "BoxTra");
            BgImg = c.Q<Image>(6, "BgImg");
            CanvasGroup = c.Q<CanvasGroup>(7, "CanvasGroup");
        }
        #endregion

        private void Awake()
        {
            InitUI();

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

        public string Title { get => TitleText.text; set => TitleText.text = value; }
        public string Content { get => ContentText.text; set => ContentText.text = value; }
        public Button.ButtonClickedEvent OkClickedEvent => OkBtn.onClick;
    }
}