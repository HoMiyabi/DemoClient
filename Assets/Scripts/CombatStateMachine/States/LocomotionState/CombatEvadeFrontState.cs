/*using UnityEngine.InputSystem;

namespace Kirara
{
    public class CombatEvadeFrontState : CombatLocomotionState
    {

        public override void OnEnter()
        {
            base.OnEnter();

            ch.PlayAction(ActionName.Evade_Front, 0.15f, () =>
            {
                sm.idle.Goto();
            });
        }

        public override void InputActionStarted(InputAction.CallbackContext ctx)
        {
            base.InputActionStarted(ctx);
            if (ctx.action.id == PlayerSystem.Instance.input.Combat.BaseAttack.id)
            {
                sm.attackRush.Goto();
            }
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
                sm.end.Goto(null);
            }
        }

        public override void Update()
        {
            base.Update();
            CharacterRotation(false);
        }
    }
}*/