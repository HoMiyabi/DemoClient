using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UINumSlider : MonoBehaviour
    {
        private Button DecreaseBtn;
        private TextMeshProUGUI MinText;
        private Slider Slider;
        private TextMeshProUGUI MaxText;
        private Button IncreaseBtn;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            DecreaseBtn = c.dict["DecreaseBtn"] as Button;
            MinText = c.dict["MinText"] as TextMeshProUGUI;
            Slider = c.dict["Slider"] as Slider;
            MaxText = c.dict["MaxText"] as TextMeshProUGUI;
            IncreaseBtn = c.dict["IncreaseBtn"] as Button;
        }

        private int value;
        public int Value
        {
            get => value;
            private set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnValueChanged?.Invoke(value);
                }
            }
        }
        public event Action<int> OnValueChanged;

        private int minValue;
        public int MinValue
        {
            get => minValue;
            set
            {
                minValue = value;
                MinText.text = value.ToString();
                Slider.minValue = value;
            }
        }

        private int maxValue;
        public int MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                MaxText.text = value.ToString();
                Slider.maxValue = value;
            }
        }

        private void Awake()
        {
            InitUI();

            DecreaseBtn.onClick.AddListener(() =>
            {
                if (Value > minValue)
                {
                    Value--;
                }
            });

            IncreaseBtn.onClick.AddListener(() =>
            {
                if (Value < maxValue)
                {
                    Value++;
                }
            });
        }

        public UINumSlider Set(int minValue, int maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;

            value = minValue;
            Slider.SetValueWithoutNotify(minValue);

            Slider.onValueChanged.AddListener(v => OnValueChanged?.Invoke((int)v));

            return this;
        }
    }
}