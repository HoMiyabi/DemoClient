// using System;
//
// namespace Kirara.Manager
// {
//     public class GameInputManager : UnitySingleton<GameInputManager>
//     {
//         public GameInput input { get; private set; }
//
//         protected override void Awake()
//         {
//             base.Awake();
//             input = new GameInput();
//         }
//
//         private void OnEnable()
//         {
//             input.Combat.Enable();
//         }
//
//         private void OnDisable()
//         {
//             input.Combat.Disable();
//         }
//     }
// }