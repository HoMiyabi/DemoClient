/*namespace Kirara
{
    public class CombatUltimateStartState : CombatAttackState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            ch.PlayAction(ActionName.SwitchIn_Attack_Ex_Start, 0f, () =>
            {
                sm.ultimate.Goto();
            });
        }
    }
}*/