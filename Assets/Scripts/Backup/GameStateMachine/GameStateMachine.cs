// using System;
// using Kirara.GameStateMachine.States;
//
// namespace Kirara.GameStateMachine
// {
//     public class GameStateMachine : StateMachine<GameState>
//     {
//         public InGameState inGame { get; private set; }
//         public UIPanelState uiPanel { get; private set; }
//
//         private void Awake()
//         {
//             inGame = new InGameState();
//             uiPanel = new UIPanelState();
//             Init();
//         }
//
//         public void Init()
//         {
//             inGame.Init(this);
//             uiPanel.Init(this);
//         }
//     }
// }