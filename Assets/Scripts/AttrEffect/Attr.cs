using System;
using System.Collections.Generic;
using cfg.main;
using UnityEngine;

namespace Kirara.AttrEffect
{
    public class Attr
    {
        public EAttrType type;

        private float baseValue;
        public float BaseValue
        {
            get => baseValue;
            set
            {
                if (Mathf.Approximately(baseValue, value)) return;

                baseValue = value;
                OnBaseValueChanged?.Invoke(value);
            }
        }
        public event Action<float> OnBaseValueChanged;

        private float currentValue;
        public float CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
            }
        }

        public readonly List<Modifier> modifiers = new();
        public AttrEffect ae;
        public float deltaValue;

        public Attr(EAttrType type, float defaultValue)
        {
            this.type = type;
            baseValue = defaultValue;
            currentValue = defaultValue;
        }

        protected void Clear()
        {
            deltaValue = 0f;
        }

        protected void Apply()
        {
            foreach (var modifier in modifiers)
            {
                modifier.Apply(this);
            }
        }

        protected float Calc()
        {
            if ((int)type % 100 == 0)
            {
                // 为一级属性
                int i = (int)type;
                float a = ae.EvaluateAttr((EAttrType)(i + 1));
                float b = ae.EvaluateAttr((EAttrType)(i + 2));
                float c = ae.EvaluateAttr((EAttrType)(i + 3));
                CurrentValue = BaseValue + deltaValue + a * (1f + b) + c;
            }
            else
            {
                // 为二级属性
                CurrentValue = BaseValue + deltaValue;
            }
            return CurrentValue;
        }

        public float Evaluate()
        {
            Clear();
            Apply();
            return Calc();
        }
    }
}