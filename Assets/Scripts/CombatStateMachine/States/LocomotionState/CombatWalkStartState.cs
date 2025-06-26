/*using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Kirara
{
    public class CombatWalkStartState : CombatLocomotionState
    {
        private CancellationTokenSource cts;

        public override void OnExit()
        {
            base.OnExit();
            cts.Cancel();
        }

        public override void OnEnter()
        {
            base.OnEnter();

            ch.PlayAction(ActionName.Walk_Start, 0.15f);

            float toWalkDuration = 0.15f;
            cts = new CancellationTokenSource();
            ScheduleToWalk(toWalkDuration, cts.Token).Forget();
        }

        private async UniTaskVoid ScheduleToWalk(float toWalkDuration, CancellationToken ct)
        {
            await UniTask.WaitForSeconds(toWalkDuration, false, PlayerLoopTiming.Update, ct);
            if (ct.IsCancellationRequested)
            {
                return;
            }
            sm.walk.Goto();
        }

        public override void Update()
        {
            base.Update();

            CharacterRotation(false);
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
                sm.evadeFront.Goto();
            }
        }

        public override void InputActionCanceled(InputAction.CallbackContext ctx)
        {
            base.InputActionCanceled(ctx);
            if (ctx.action.id == PlayerSystem.Instance.input.Combat.Move.id)
            {
                sm.idle.Goto();
            }
        }
    }
}*/