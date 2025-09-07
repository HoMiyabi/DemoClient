// using System.Collections.Generic;
//
// namespace Kirara
// {
//     public static class ActionPriority
//     {
//         public const int Level1 = 100;
//         public const int Level2 = 200;
//         public const int Level3 = 300;
//         public const int Level4 = 400;
//         public const int Level5 = 500;
//         public const int Level6 = 600;
//
//         private static readonly Dictionary<EActionState, int> _priority = new()
//         {
//             {EActionState.End, Level1},
//             {EActionState.Idle, Level1},
//
//             {EActionState.Walk, Level2},
//             {EActionState.Run, Level2},
//             {EActionState.Switch, Level2},
//
//             {EActionState.Attack_Normal, Level3},
//
//             {EActionState.Dodge_Back, Level4},
//             {EActionState.Dodge_Front, Level4},
//
//             {EActionState.Attack_Special, Level5},
//             {EActionState.Attack_Ex_Special, Level5},
//
//             {EActionState.Attack_ParryAid, Level6},
//             {EActionState.Attack_Ultimate, Level6}
//         };
//
//         public static int Get(EActionState state)
//         {
//             return _priority[state];
//         }
//     }
// }