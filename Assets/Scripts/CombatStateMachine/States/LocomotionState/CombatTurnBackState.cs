/*using UnityEngine.InputSystem;

namespace Kirara
{
    public class CombatTurnBackState : CombatLocomotionState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            ch.PlayAction(ActionName.TurnBack, 0.15f);
        }

        public override void OnToEnd()
        {
            base.OnToEnd();
            if (PlayerSystem.Instance.input.Combat.Move.phase == InputActionPhase.Started)
            {
                sm.run.Goto();
            }
            else
            {
                sm.end.Goto("Run_End");
            }
        }
    }
}*/