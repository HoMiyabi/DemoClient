using System.ComponentModel;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [DisplayName("启用格挡支援")]
    public class EnableParryAidNotifyState : ActionNotifyState
    {
        public override void NotifyBegin(ActionCtrl actionCtrl)
        {
            var roleCtrl = actionCtrl.GetComponent<RoleCtrl>();
            if (roleCtrl)
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
            var roleCtrl = actionCtrl.GetComponent<RoleCtrl>();
            if (roleCtrl)
            {
                NetFn.Send(new MsgRoleSetParry()
                {
                    RoleId = roleCtrl.Role.Id,
                    Parrying = false
                });
            }
            // ch.EnableParryAid = false;
        }
    }
}