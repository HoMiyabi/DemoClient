// using UnityEngine;
//
// namespace Kirara.GameStateMachine.States
// {
//     public class InGameState : GameState
//     {
//         public override void OnEnter()
//         {
//             Cursor.visible = false;
//             Cursor.lockState = CursorLockMode.Locked;
//
//             UIMgr.Instance.onViewPushed += ToUIPanel;
//
//             if (CombatSceneManager.Instance != null)
//             {
//                 CombatSceneManager.Instance.gameObject.SetActive(true);
//             }
//         }
//
//         public override void OnExit()
//         {
//             Cursor.visible = true;
//             Cursor.lockState = CursorLockMode.None;
//
//             UIMgr.Instance.onViewPushed -= ToUIPanel;
//
//             if (CombatSceneManager.Instance != null)
//             {
//                 CombatSceneManager.Instance.gameObject.SetActive(false);
//             }
//         }
//
//         private void ToUIPanel()
//         {
//             sm.ChangeState(sm.uiPanel);
//         }
//     }
// }