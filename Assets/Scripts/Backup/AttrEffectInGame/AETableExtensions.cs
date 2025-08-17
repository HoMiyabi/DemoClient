// using System.Linq;
// using cfg.main;
// using Kirara.AttrEffect;
//
// namespace Kirara
// {
//     public static class AETableExtensions
//     {
//         public static Ability GetRuntime(this AbilityConfig config)
//         {
//             return null;
//         }
//
//         public static Effect GetRuntime(this EffectConfig config)
//         {
//             return new Effect(
//                 config.Name,
//                 (EffectDurationPolicy)config.DurationPolicy,
//                 config.Duration,
//                 config.Modifiers
//                     .Select(x => x.GetRuntime() as Modifier)
//                     .ToList(),
//                 config.StackLimit);
//         }
//
//         public static Modifier GetRuntime(this ModifierConfig config)
//         {
//             return new Modifier(config.AttrType, config.DeltaValue);
//         }
//     }
// }