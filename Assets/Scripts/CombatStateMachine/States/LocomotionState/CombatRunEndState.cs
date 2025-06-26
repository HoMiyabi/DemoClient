/*using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kirara
{
    public class CombatRunEndState : CombatLocomotionState
    {
        public Vector2 toRunEndMoveInput;

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void Goto()
        {
            throw new NotImplementedException();
        }

        public void Goto(Vector2 toRunEndMoveInput)
        {
            sm.Exit();
            this.toRunEndMoveInput = toRunEndMoveInput;
            sm.Enter(this);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            ch.PlayAction(ActionName.Run_End, 0.15f, () =>
            {
                sm.idle.Goto();
            });
        }

        public override void InputActionStarted(InputAction.CallbackContext ctx)
        {
            base.InputActionStarted(ctx);
            if (ctx.action.id == PlayerSystem.Instance.input.Combat.BaseAttack.id)
            {
                sm.attackNormal.Goto(1, false);
            }
            else if (ctx.action.id == PlayerSystem.Instance.input.Combat.Dodge.id)
            {
                sm.evadeBack.Goto();
            }
            else if (ctx.action.id == PlayerSystem.Instance.input.Combat.Move.id)
            {
                var curMoveInput = ctx.ReadValue<Vector2>();
                if (Vector2.Dot(curMoveInput, toRunEndMoveInput) < -0.9f)
                {
                    sm.turnBack.Goto();
                }
                else
                {
                    sm.walkStart.Goto();
                }
            }
        }
    }
}*/