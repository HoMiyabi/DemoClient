using System.ComponentModel;
using cfg.main;

namespace Kirara.TimelineAction
{
    [DisplayName("消耗能量通知")]
    public class ConsumeEnergyNotify : ActionNotify
    {
        public float cost;

        public override void Notify(ActionPlayer player)
        {
            base.Notify(player);
            if (player.TryGetComponent<RoleCtrl>(out var roleCtrl))
            {
                roleCtrl.ConsumeEnergy(cost);
            }
        }
    }
}