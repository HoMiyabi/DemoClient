/*using cfg.main;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kirara
{
    public class CombatEvadeBackState : CombatLocomotionState
    {
        private readonly Collider[] hitCols = new Collider[10];

        public override void OnEnter()
        {
            base.OnEnter();

            ch.PlayAction(ActionName.Evade_Back, 0.15f, () =>
            {
                sm.idle.Goto();
            });

            GetCharacterControllerPoints(ch.CharacterController, ch.transform,
                out var p0, out var p1, out float radius);

            int size = Physics.OverlapCapsuleNonAlloc(p0, p1, radius, hitCols, LayerMask.GetMask("DodgeJudge"));
            if (size > 0)
            {
                Debug.Log("闪避成功");
            }

            ch.ChModel.ae.GetAttr(EAttrType.Invincible).BaseValue = 1f;
        }

        private void GetCharacterControllerPoints(CharacterController chCtl, Transform transform,
            out Vector3 p0, out Vector3 p1, out float radius)
        {
            var offset = new Vector3(0f, chCtl.height * 0.5f - chCtl.radius, 0f);
            var p0Local = chCtl.center - offset;
            var p1Local = chCtl.center + offset;
            p0 = transform.TransformPoint(p0Local);
            p1 = transform.TransformPoint(p1Local);
            radius = chCtl.radius;
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
            sm.end.Goto(null);

            Debug.Log("Should not go here");

            ch.ChModel.ae.GetAttr(EAttrType.Invincible).BaseValue = 0f;
        }
    }
}*/