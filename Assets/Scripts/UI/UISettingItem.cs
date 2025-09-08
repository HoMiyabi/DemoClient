using System;
using UnityEngine;

namespace Kirara.UI
{
    public class UISettingItem : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI NameText;
        private TMPro.TextMeshProUGUI ValueText;
        private UnityEngine.UI.Slider Slider;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b     = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            NameText  = b.Q<TMPro.TextMeshProUGUI>(0, "NameText");
            ValueText = b.Q<TMPro.TextMeshProUGUI>(1, "ValueText");
            Slider    = b.Q<UnityEngine.UI.Slider>(2, "Slider");
        }
        #endregion

        public void Set(string settingName, int minValue, int maxValue, int value, Action<int> onValueChanged)
        {
            BindUI();

            NameText.text = settingName;

            ValueText.text = value.ToString();

            Slider.minValue = minValue;
            Slider.maxValue = maxValue;
            Slider.wholeNumbers = true;

            Slider.onValueChanged.RemoveAllListeners();
            Slider.onValueChanged.AddListener(v => ValueText.text = Mathf.RoundToInt(v).ToString());
            Slider.value = value;

            if (onValueChanged != null)
            {
                Slider.onValueChanged.AddListener(v => onValueChanged(Mathf.RoundToInt(v)));
            }
        }
    }
}