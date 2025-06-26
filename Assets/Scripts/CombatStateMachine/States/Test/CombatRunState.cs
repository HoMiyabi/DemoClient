// namespace Kirara
// {
//     public class CombatRunState : CombatState
//     {
//         public CombatRunState(CombatStateMachine sm, EActionState state) : base(sm, state)
//         {
//         }
//
//         public override EActionPriority Priority => EActionPriority.Run;
//
//         public override void OnEnter()
//         {
//             ch.IsRotationEnabled = true;
//             ch.IsRecenterEnabled = true;
//         }
//
//         public override void OnExit()
//         {
//             ch.IsRotationEnabled = false;
//             ch.IsRecenterEnabled = false;
//         }
//     }
// }