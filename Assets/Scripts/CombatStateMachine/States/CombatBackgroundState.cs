/*namespace Kirara
{
    public class CombatBackgroundState : CombatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            sm.ch.ActionCtrl.ActionPlayer.Stop();
            sm.ch.CharacterController.enabled = false;
            sm.ch.ChGravity.enabled = false;
            sm.gameObject.SetActive(false);
        }

        public override void OnExit()
        {
            base.OnExit();
            sm.ch.CharacterController.enabled = true;
            sm.ch.ChGravity.enabled = true;
            sm.gameObject.SetActive(true);
        }
    }
}*/