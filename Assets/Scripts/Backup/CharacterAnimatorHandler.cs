// using System;
// using UnityEngine;
//
// namespace Kirara
// {
//     /// <summary>
//     /// 方法由Animator调用
//     /// </summary>
//     public class CharacterAnimatorHandler : MonoBehaviour
//     {
//         public event Action onToEnd;
//         public event Action onEnableInputBuffer;
//         public event Action onEnableCombo;
//         public event Action onDisableCombo;
//         public event Action onAtk;
//
//         private Animator animator;
//
//         private void Awake()
//         {
//             animator = GetComponent<Animator>();
//         }
//
//         private void ToEnd()
//         {
//             if (animator.IsInTransition(0))
//             {
//                 Debug.Log("Animator transition时触发ToEnd，不执行");
//                 return;
//             }
//             onToEnd?.Invoke();
//         }
//
//         private void EnableInputBuffer()
//         {
//             onEnableInputBuffer?.Invoke();
//         }
//
//         private void EnableCombo()
//         {
//             onEnableCombo?.Invoke();
//         }
//
//         private void DisableCombo()
//         {
//             onDisableCombo?.Invoke();
//         }
//
//         private void ATK()
//         {
//             onAtk?.Invoke();
//         }
//     }
// }