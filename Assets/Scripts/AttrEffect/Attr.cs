using System;
using System.Collections.Generic;
using cfg.main;
using UnityEngine;

namespace Kirara.AttrEffect
{
    public class Attr
    {
        public int type;

        private double baseValue;
        public double BaseValue
        {
            get => baseValue;
            set
            {
                if (baseValue == value) return;

                baseValue = value;
                OnBaseValueChanged?.Invoke(value);
            }
        }
        public event Action<double> OnBaseValueChanged;

        public readonly List<ILuaAbility> abilities = new();
        public AttrSet set;

        public Attr(int type, double baseValue)
        {
            this.type = type;
            this.baseValue = baseValue;
        }

        public double Evaluate()
        {
            double delta = 0;
            foreach (var ability in abilities)
            {
                delta += ability.stackCount * ability.attrs.Get<double>(type.ToString());
            }

            if (type % 100 == 0)
            {
                // 为一级属性
                int i = type;
                double bas = set[i + 1];
                double pct = set[i + 2];
                double fix = set[i + 3];
                return BaseValue + delta + bas * (1f + pct) + fix;
            }
            // 为二级属性
            return BaseValue + delta;
        }
    }
}