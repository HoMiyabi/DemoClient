using System.ComponentModel;

namespace Kirara.TimelineAction
{
    [DisplayName("控制格挡支援通知状态")]
    public class ControlParryAidNotifyState : ActionNotifyState
    {
        public override void NotifyBegin(ActionCtrl actionCtrl)
        {
            if (actionCtrl.TryGetComponent<RoleCtrl>(out var roleCtrl))
            {
                NetFn.Send(new MsgRoleSetParry()
                {
                    RoleId = roleCtrl.Role.Id,
                    Parrying = true
                });
            }
        }

        public override void NotifyEnd(ActionCtrl actionCtrl)
        {
            if (actionCtrl.TryGetComponent<RoleCtrl>(out var roleCtrl))
            {
                NetFn.Send(new MsgRoleSetParry()
                {
                    RoleId = roleCtrl.Role.Id,
                    Parrying = false
                });
            }
        }
    }
}