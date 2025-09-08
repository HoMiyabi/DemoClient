using System.ComponentModel;

namespace Kirara.TimelineAction
{
    [DisplayName("覆盖其他动作的参数和转移的通知")]
    public class OverrideActionParamsAndTransitionNotify : ActionNotify
    {
        [TimelineActionName]
        public string actionName;

        public override void Notify(ActionCtrl actionCtrl)
        {
            if (actionCtrl.TryGetAction(actionName, out var action))
            {
                actionCtrl.OverrideAction = action;
                actionCtrl.OnSetActionParams?.Invoke(action.actionParams);
            }
        }
    }
}