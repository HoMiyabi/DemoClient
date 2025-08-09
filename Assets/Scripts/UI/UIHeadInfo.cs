using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIHeadInfo : MonoBehaviour
    {
        #region View
        private Image           LeftArrowImg;
        private TextMeshProUGUI NameText;
        private Image           RightArrowImg;
        private void InitUI()
        {
            var c         = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            LeftArrowImg  = c.Q<Image>(0, "LeftArrowImg");
            NameText      = c.Q<TextMeshProUGUI>(1, "NameText");
            RightArrowImg = c.Q<Image>(2, "RightArrowImg");
        }
        #endregion

        public string Name
        {
            get => NameText.text;
            set => NameText.text = value;
        }

        public Transform wsFollow;
        public Vector3 localPos;

        private RectTransform rectTransform;

        private bool selected;
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;

                LeftArrowImg.gameObject.SetActive(value);
                RightArrowImg.gameObject.SetActive(value);
            }
        }

        private void Awake()
        {
            InitUI();

            rectTransform = transform as RectTransform;
            Selected = false;
        }

        public UIHeadInfo Set(string text, Transform wsFollow, Vector3 localPos = default)
        {
            NameText.text = text;
            this.wsFollow = wsFollow;
            this.localPos = localPos;
            return this;
        }

        private void Update()
        {
            RectUtils.SetRectWorldPos(rectTransform, wsFollow.TransformPoint(localPos));
        }
    }
}