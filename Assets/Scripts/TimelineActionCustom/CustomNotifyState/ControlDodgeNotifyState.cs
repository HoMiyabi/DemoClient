using System.ComponentModel;

namespace Kirara.TimelineAction
{
    [DisplayName("控制闪避通知状态")]
    public class ControlDodgeNotifyState : ActionNotifyState
    {
        public override void NotifyBegin(ActionCtrl actionCtrl)
        {
            base.NotifyBegin(actionCtrl);
            if (actionCtrl.TryGetComponent<RoleCtrl>(out var roleCtrl))
            {
                NetFn.Send(new MsgRoleSetDodge
                {
                    RoleId = roleCtrl.Role.Id,
                    Dodging = true
                });
            }
        }

        public override void NotifyEnd(ActionCtrl actionCtrl)
        {
            base.NotifyEnd(actionCtrl);
            if (actionCtrl.TryGetComponent<RoleCtrl>(out var roleCtrl))
            {
                NetFn.Send(new MsgRoleSetDodge
                {
                    RoleId = roleCtrl.Role.Id,
                    Dodging = false
                });
            }
        }
    }
}