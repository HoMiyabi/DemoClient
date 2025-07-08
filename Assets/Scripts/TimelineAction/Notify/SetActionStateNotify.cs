using System.ComponentModel;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [DisplayName("设置动作状态通知")]
    public class SetActionStateNotify : ActionNotify
    {
        public EActionState actionState;
        public ActionParams actionParams;

        public override void Notify(ActionPlayer player)
        {
            var actionCtrl = player.GetComponent<ActionCtrl1>();
            if (actionCtrl == null)
            {
                Debug.LogWarning("ActionCtrl is null");
                return;
            }
            actionCtrl.State = actionState;

            var chCtrl = player.GetComponent<RoleCtrl>();
            if (chCtrl == null)
            {
                Debug.LogWarning("ChCtrl is null");
                return;
            }
            chCtrl.SetActionParams(actionParams);
        }
    }
}