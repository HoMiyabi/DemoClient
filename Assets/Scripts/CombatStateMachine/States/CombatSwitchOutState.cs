/*using DG.Tweening;

namespace Kirara
{
    public class CombatSwitchOutState : CombatState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            ch.PlayAction(ActionName.SwitchOut_Normal, 0f, () =>
            {
                sm.background.Goto();
            });

            ch.CharacterController.enabled = false;
            ch.ChGravity.enabled = false;
            ch.transform.DOKill();
        }

        public override void OnExit()
        {
            base.OnExit();
            ch.CharacterController.enabled = true;
            ch.ChGravity.enabled = true;
        }
    }
}*/