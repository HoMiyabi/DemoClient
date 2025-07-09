// using System.Collections.Generic;
// using cfg.main;
// using UnityEngine;
//
// namespace Kirara.AttrEffect
// {
//     public enum EffectDurationPolicy
//     {
//         Instant,
//         Infinite,
//         HasDuration
//     }
//
//     public enum RefreshPolicy
//     {
//
//     }
//
//     public class Effect
//     {
//         public readonly string name;
//         public AttrEffect ae;
//
//         public readonly EffectDurationPolicy durationPolicy;
//         public readonly float duration;
//         public readonly List<float> remainingTimes = new(); // 每层效果单独结算持续时间
//
//         public readonly List<Modifier> modifiers;
//
//         // Stacking
//         public int StackLimit { get; private set; }
//         public int StackCount { get; private set; }
//
//         public Effect(string name, EffectDurationPolicy durationPolicy, float duration, List<Modifier> modifiers,
//             int stackLimit = 1)
//         {
//             this.name = name;
//             this.durationPolicy = durationPolicy;
//             this.duration = duration;
//
//             foreach (var modifier in modifiers)
//             {
//                 modifier.effect = this;
//             }
//             this.modifiers = modifiers;
//
//             StackLimit = stackLimit;
//             if (StackLimit < 1)
//             {
//                 Debug.LogError($"{nameof(Effect)} {name}的{nameof(StackLimit)}必须大于等于1");
//             }
//         }
//
//         public void Apply(Dictionary<EAttrType, Attr> attrs)
//         {
//             if (StackCount == 0)
//             {
//                 foreach (var modifier in modifiers)
//                 {
//                     if (attrs.TryGetValue(modifier.AttrType, out var attr))
//                     {
//                         attr.modifiers.Add(modifier);
//                     }
//                     else
//                     {
//                         Debug.LogWarning($"{nameof(Attr)} {modifier.AttrType}不存在");
//                     }
//                 }
//             }
//             if (StackCount < StackLimit)
//             {
//                 if (durationPolicy == EffectDurationPolicy.HasDuration)
//                 {
//                     remainingTimes.Add(duration);
//                 }
//                 StackCount++;
//             }
//             else
//             {
//                 if (durationPolicy == EffectDurationPolicy.HasDuration)
//                 {
//                     // 到达上限后刷新剩余时间最小层的持续时间
//                     int idx = 0;
//                     float min = remainingTimes[0];
//                     for (int i = 1; i < remainingTimes.Count; i++)
//                     {
//                         if (remainingTimes[i] < min)
//                         {
//                             min = remainingTimes[i];
//                             idx = i;
//                         }
//                     }
//                     remainingTimes[idx] = duration;
//                 }
//             }
//         }
//
//         public void Remove(Dictionary<EAttrType, Attr> attrs)
//         {
//             foreach (var modifier in modifiers)
//             {
//                 if (attrs.TryGetValue(modifier.AttrType, out var attr))
//                 {
//                     attr.modifiers.Remove(modifier);
//                 }
//                 else
//                 {
//                     Debug.LogWarning($"{nameof(Attr)} {modifier.AttrType}不存在");
//                 }
//             }
//         }
//
//         public virtual bool Update()
//         {
//             if (durationPolicy == EffectDurationPolicy.HasDuration)
//             {
//                 for (int i = 0; i < remainingTimes.Count;)
//                 {
//                     remainingTimes[i] -= Time.deltaTime;
//                     if (remainingTimes[i] < 0)
//                     {
//                         remainingTimes.RemoveAt(i);
//                         StackCount--;
//                     }
//                     else
//                     {
//                         i++;
//                     }
//                 }
//                 if (StackCount == 0)
//                 {
//                     return true;
//                 }
//             }
//             return false;
//         }
//     }
// }