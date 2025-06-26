// namespace Kirara
// {
//     public class CombatDodgeFrontState : CombatState
//     {
//         public CombatDodgeFrontState(CombatStateMachine sm, EActionState state) : base(sm, state)
//         {
//         }
//
//         public override EActionPriority Priority => EActionPriority.Dodge_Front;
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