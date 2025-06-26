// namespace Kirara.GameStateMachine.States
// {
//     public class UIPanelState : GameState
//     {
//         public override void OnEnter()
//         {
//             UIMgr.Instance.onViewPopped += ToInGame;
//         }
//
//         public override void OnExit()
//         {
//             UIMgr.Instance.onViewPopped -= ToInGame;
//         }
//
//         private void ToInGame()
//         {
//             if (UIMgr.Instance.NormalPanelCount == 0)
//             {
//                 sm.ChangeState(sm.inGame);
//             }
//         }
//     }
// }