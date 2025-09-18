using System.ComponentModel;

namespace Kirara.TimelineAction
{
    [DisplayName("消耗能量通知")]
    public class ConsumeEnergyNotify : ActionNotify
    {
        public float cost;

        public override void Notify(ActionCtrl actionCtrl)
        {
            base.Notify(actionCtrl);
            if (actionCtrl.TryGetComponent<RoleCtrl>(out var roleCtrl))
            {
                roleCtrl.ConsumeEnergy(cost);
            }
        }
    }
}