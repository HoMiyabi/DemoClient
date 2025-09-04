using System.ComponentModel;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [DisplayName("启用格挡支援")]
    public class EnableParryAidNotifyState : ActionNotifyState
    {
        public override void NotifyBegin(ActionPlayer player)
        {
            var roleCtrl = player.GetComponent<RoleCtrl>();
            if (roleCtrl)
            {
                NetFn.Send(new MsgRoleSetParry()
                {
                    RoleId = roleCtrl.Role.Id,
                    Parrying = true
                });
            }
        }

        public override void NotifyEnd(ActionPlayer player)
        {
            var roleCtrl = player.GetComponent<RoleCtrl>();
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