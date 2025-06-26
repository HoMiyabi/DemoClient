using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class AlterDialogPanel : BasePanel
    {
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
            c.Init();
            BoxBgImg = c.Q<Image>("BoxBgImg");
            TitleText = c.Q<TextMeshProUGUI>("TitleText");
            ContentText = c.Q<TextMeshProUGUI>("ContentText");
            OkBtn = c.Q<Button>("OkBtn");
            CloseBtn = c.Q<Button>("CloseBtn");
            BoxTra = c.Q<RectTransform>("BoxTra");
            BgImg = c.Q<Image>("BgImg");
            CanvasGroup = c.Q<CanvasGroup>("CanvasGroup");
        }

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