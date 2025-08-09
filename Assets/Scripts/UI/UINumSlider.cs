using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UINumSlider : MonoBehaviour
    {
        #region View
        private Button          DecreaseBtn;
        private TextMeshProUGUI MinText;
        private Slider          Slider;
        private TextMeshProUGUI MaxText;
        private Button          IncreaseBtn;
        private void InitUI()
        {
            var c       = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            DecreaseBtn = c.Q<Button>(0, "DecreaseBtn");
            MinText     = c.Q<TextMeshProUGUI>(1, "MinText");
            Slider      = c.Q<Slider>(2, "Slider");
            MaxText     = c.Q<TextMeshProUGUI>(3, "MaxText");
            IncreaseBtn = c.Q<Button>(4, "IncreaseBtn");
        }
        #endregion

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