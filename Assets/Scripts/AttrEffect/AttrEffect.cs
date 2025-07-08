using System.Collections.Generic;
using cfg.main;
using UnityEngine;

namespace Kirara.AttrEffect
{
    /// <summary>
    /// 属性效果
    /// </summary>
    public class AttrEffect
    {
        public readonly Dictionary<EAttrType, Attr> attrs = new();
        public readonly Dictionary<string, Effect> effects = new();
        public readonly Dictionary<string, Ability> abilities = new();

        private readonly List<Effect> effectsToRemove = new();

        #region Update

        public void Update()
        {
            UpdateEffects();
        }

        private void UpdateEffects()
        {
            effectsToRemove.Clear();
            foreach (var effect in effects.Values)
            {
                bool shouldRemove = effect.Update();
                if (shouldRemove)
                {
                    effectsToRemove.Add(effect);
                }
            }
            foreach (var effect in effectsToRemove)
            {
                effects.Remove(effect.name);
            }
        }

        #endregion

        #region 属性 Attr

        public Attr AddAttr(Attr attr)
        {
            attrs.Add(attr.type, attr);
            attr.ae = this;
            return attr;
        }

        public bool TryAddAttr(Attr attr)
        {
            if (attrs.TryAdd(attr.type, attr))
            {
                attr.ae = this;
                return true;
            }
            return false;
        }

        public Attr GetAttr(EAttrType type)
        {
            if (!attrs.TryGetValue(type, out var attr))
            {
                Debug.LogWarning($"{nameof(Attr)} {type}不存在");
                return null;
            }
            return attr;
        }

        public float EvaluateAttr(EAttrType type)
        {
            if  (!attrs.TryGetValue(type, out var attr))
            {
                return 0;
            }
            return attr.Evaluate();
        }

        #endregion

        // #region 效果 Effect
        //
        // public void ApplyEffect(Effect effect)
        // {
        //     if (effect.durationPolicy == EffectDurationPolicy.HasDuration && effect.duration <= 0)
        //     {
        //         Debug.LogWarning($"{nameof(Effect)} {effect.GetType().Name}剩余时间小于等于0");
        //         return;
        //     }
        //     effect.ae = this;
        //     effect.Apply(attrs);
        //     effects.TryAdd(effect.name, effect);
        // }
        //
        // public bool RemoveEffect(string effectName)
        // {
        //     if (!effects.Remove(effectName, out var effect)) return false;
        //     effect.ae = null;
        //     effect.Remove(attrs);
        //     return true;
        // }
        //
        // #endregion

        #region 能力 Ability
        public void AddAbility(Ability ability)
        {
            if (!abilities.TryAdd(ability.name, ability))
            {
                Debug.LogWarning($"{nameof(Ability)} {ability.name}已存在");
                return;
            }
            // ability.onAdded();
        }

        public void RemoveAbility(string abilityName)
        {
            if (!abilities.Remove(abilityName, out var ability))
            {
                Debug.LogWarning($"{nameof(Ability)} {abilityName}不存在");
            }
        }

        #endregion
    }
}