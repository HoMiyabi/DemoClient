/*using UnityEngine.InputSystem;

namespace Kirara
{
    public class CombatSwitchInState : CombatLocomotionState
    {
        private bool hasBaseAttackCommand;

        public override void OnEnter()
        {
            base.OnEnter();

            ch.PlayAction(ActionName.SwitchIn_Normal, 0f, () =>
            {
                sm.idle.Goto();
            });

            hasBaseAttackCommand = false;

            sm.ch.CharacterController.enabled = false;
            sm.ch.ChGravity.enabled = false;

            if (PlayerSystem.Instance.input.Combat.Move.phase == InputActionPhase.Started)
            {
                sm.run.Goto();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            sm.ch.CharacterController.enabled = true;
            sm.ch.ChGravity.enabled = true;
        }

        public override void InputActionStarted(InputAction.CallbackContext ctx)
        {
            base.InputActionStarted(ctx);
            if (ctx.action.id == PlayerSystem.Instance.input.Combat.Move.id)
            {
                sm.run.Goto();
            }
            else if (ctx.action.id == PlayerSystem.Instance.input.Combat.BaseAttack.id)
            {
                hasBaseAttackCommand = true;
            }
        }

        public override void OnToEnd()
        {
            base.OnToEnd();
            if (hasBaseAttackCommand)
            {
                sm.attackNormal.Goto(1, false);
            }
            else
            {
                sm.end.Goto(null);
            }
        }
    }
}*/