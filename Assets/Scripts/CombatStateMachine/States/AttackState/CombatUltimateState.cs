/*namespace Kirara
{
    public class CombatUltimateState : CombatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            ch.PlayAction(ActionName.SwitchIn_Attack_Ex, 0f, OnEnd);

            ch.ChGravity.enabled = false;
        }

        private void OnEnd()
        {
            string skillName = "SwitchIn_Attack_Ex_End";
            sm.end.Goto(skillName);
        }

        public override void OnExit()
        {
            base.OnExit();
            sm.ch.ChGravity.enabled = true;
        }
    }
}*/