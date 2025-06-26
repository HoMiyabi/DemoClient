// namespace Kirara
// {
//     public class CombatWalkState : CombatState
//     {
//         public CombatWalkState(CombatStateMachine sm, EActionState state) : base(sm, state)
//         {
//         }
//
//         public override EActionPriority Priority => EActionPriority.Walk;
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