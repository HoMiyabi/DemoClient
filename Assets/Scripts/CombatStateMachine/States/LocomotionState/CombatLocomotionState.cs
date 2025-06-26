/*using UnityEngine.InputSystem;

namespace Kirara
{
    public class CombatLocomotionState : CombatState
    {
        public override void InputActionStarted(InputAction.CallbackContext ctx)
        {
            base.InputActionStarted(ctx);

            if (ctx.action.id == PlayerSystem.Instance.input.Combat.SwitchCharactersPrevious.id)
            {
                PlayerSystem.Instance.SwitchCharacterPrev();
            }
            else if (ctx.action.id == PlayerSystem.Instance.input.Combat.SwitchCharactersNext.id)
            {
                // Debug.Log($"Switch Time.frameCount={Time.frameCount} " +
                //           $"sm.gameObject={sm.gameObject}");
                PlayerSystem.Instance.SwitchCharacterNext();
            }
        }

        public override void ScheduleSwitchOut()
        {
            sm.switchOut.Goto();
        }
    }
}*/