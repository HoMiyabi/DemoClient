using cfg.main;

namespace Kirara.TimelineAction
{
    public class ConsumeEnergyNotify : ActionNotify
    {
        public float cost;

        public override void Notify(ActionPlayer player)
        {
            base.Notify(player);
            var roleCtrl = player.GetComponent<RoleCtrl>();
            if (roleCtrl == null) return;
            roleCtrl.Role.Set[EAttrType.CurrEnergy] -= cost;
        }
    }
}