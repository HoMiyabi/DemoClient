// using UnityEngine;
//
// namespace Kirara
// {
//     public static class AniId
//     {
//         public static readonly int Empty = Animator.StringToHash("Empty");
//         public static readonly int Idle = Animator.StringToHash("Idle");
//         public static readonly int Walk_Start = Animator.StringToHash("Walk_Start");
//         public static readonly int Walk = Animator.StringToHash("Walk");
//         public static readonly int Run_End = Animator.StringToHash("Run_End");
//         public static readonly int Run = Animator.StringToHash("Run");
//         public static readonly int Evade_Front = Animator.StringToHash("Evade_Front");
//         public static readonly int Evade_Back = Animator.StringToHash("Evade_Back");
//         public static readonly int TurnBack = Animator.StringToHash("TurnBack");
//
//         public static readonly int Attack_Rush = Animator.StringToHash("Attack_Rush");
//         public static readonly int Attack_Rush_End = Animator.StringToHash("Attack_Rush_End");
//
//         private static readonly int[] segmentToAttackNormal = new int[9];
//         private static readonly int[] segmentToAttackNormalEnd = new int[9];
//         private static readonly int[] segmentToAttackNormalPerfect = new int[9];
//         private static readonly int[] segmentToAttackNormalPerfectEnd = new int[9];
//
//         static AniId()
//         {
//             for (int i = 0; i < 9; i++)
//             {
//                 segmentToAttackNormal[i] = Animator.StringToHash($"Attack_Normal_0{i + 1}");
//                 segmentToAttackNormalEnd[i] = Animator.StringToHash($"Attack_Normal_0{i + 1}_End");
//                 segmentToAttackNormalPerfect[i] = Animator.StringToHash($"Attack_Normal_0{i + 1}_Perfect");
//                 segmentToAttackNormalPerfectEnd[i] = Animator.StringToHash($"Attack_Normal_0{i + 1}_Perfect_End");
//             }
//         }
//
//         public static int GetAttackNormal(int segment, bool isEnd, bool isPerfect)
//         {
//             if (isEnd && isPerfect)
//             {
//                 return segmentToAttackNormalPerfectEnd[segment - 1];
//             }
//             if (isEnd)
//             {
//                 return segmentToAttackNormalEnd[segment - 1];
//             }
//             if (isPerfect)
//             {
//                 return segmentToAttackNormalPerfect[segment - 1];
//             }
//             return segmentToAttackNormal[segment - 1];
//         }
//     }
// }