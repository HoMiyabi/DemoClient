using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UISettingItem : MonoBehaviour
    {
        private TextMeshProUGUI NameText;
        private TextMeshProUGUI ValueText;
        private Slider Slider;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            NameText = c.dict["NameText"] as TextMeshProUGUI;
            ValueText = c.dict["ValueText"] as TextMeshProUGUI;
            Slider = c.dict["Slider"] as Slider;
        }

        private void Awake()
        {
            InitUI();
        }

        public UISettingItem Set(string settingName, int minValue, int maxValue, int value, Action<int> onValueChanged)
        {
            NameText.text = settingName;

            Slider.minValue = minValue;
            Slider.maxValue = maxValue;
            Slider.wholeNumbers = true;

            Slider.onValueChanged.RemoveAllListeners();
            Slider.onValueChanged.AddListener((v) => ValueText.text = ((int)v).ToString());
            Slider.value = value;

            if (onValueChanged != null)
            {
                Slider.onValueChanged.AddListener((v) => onValueChanged((int)v));
            }

            return this;
        }
    }
}