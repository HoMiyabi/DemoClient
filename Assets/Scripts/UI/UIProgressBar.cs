using UnityEngine;

namespace Kirara.UI
{
    public class UIProgressBar : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI Text;
        private UnityEngine.UI.Image  Front;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Text  = b.Q<TMPro.TextMeshProUGUI>(0, "Text");
            Front = b.Q<UnityEngine.UI.Image>(1, "Front");
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
                _progress = value;
                Text.text = value.ToString("P0");
                Front.fillAmount = value;
            }
        }
    }
}