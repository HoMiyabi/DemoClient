using UnityEngine;

namespace Kirara.UI
{
    public class UIProgressBar : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI Text;
        private UnityEngine.UI.Image  BarImg;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b  = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Text   = b.Q<TMPro.TextMeshProUGUI>(0, "Text");
            BarImg = b.Q<UnityEngine.UI.Image>(1, "BarImg");
        }
        #endregion

        private void Awake()
        {
            BindUI();

            Progress = 0f;
        }

        private float _progress;
        public float Progress
        {
            get => _progress;
            set
            {
                Text.text = value.ToString("P0");
                BarImg.fillAmount = value;
            }
        }
    }
}