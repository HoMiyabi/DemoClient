using System.ComponentModel;

namespace Kirara.TimelineAction
{
    [DisplayName("命中停顿通知")]
    public class HitstopNotify : ActionNotify
    {
        [TimeField(60)]
        public float duration = 0.05f;
        public float animationSpeed;

        public override void Notify(ActionCtrl actionCtrl)
        {
            var roleCtrl = actionCtrl.GetComponent<RoleCtrl>();
            if (!roleCtrl) return;
            roleCtrl.TriggerHitstopIfHitMonster(duration, animationSpeed);
        }
    }
}