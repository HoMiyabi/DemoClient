using System.ComponentModel;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [DisplayName("覆盖其他动作的参数和转移的通知")]
    public class OverrideActionParamsAndTransitionNotify : ActionNotify
    {
        public string actionName;

        public override void Notify(ActionPlayer player)
        {
            var actionCtrl = player.GetComponent<ActionCtrl>();
            if (actionCtrl == null)
            {
                Debug.LogWarning("ActionCtrl is null");
                return;
            }
            var action = actionCtrl.ActionDict[actionName];
            actionCtrl.OverrideAction = action;
            actionCtrl.onSetActionParams?.Invoke(action.actionParams);
        }
    }
}