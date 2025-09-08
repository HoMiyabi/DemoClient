using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

namespace Kirara.TimelineAction
{
    [DisplayName("指令转移")]
    public class CommandTransitionNotifyState : ActionNotifyState
    {
        public float inputBufferDuration = 0.1f;
        [FormerlySerializedAs("cancelInfo")] public CommandTransitionInfo commandTransition;

        public override void NotifyBegin(ActionCtrl actionCtrl)
        {
            double time = Time.timeAsDouble;
            for (int i = actionCtrl.InputBuffer.Count - 1; i >= 0; i--)
            {
                var item = actionCtrl.InputBuffer[i];
                if (!item.used &&
                    item.command == commandTransition.command &&
                    item.phase == commandTransition.phase &&
                    item.time >= time - inputBufferDuration &&
                    actionCtrl.IsActionExecutableInternal(commandTransition.actionName))
                {
                    item.used = true;
                    actionCtrl.InputBuffer[i] = item;
                    actionCtrl.PlayAction(commandTransition.actionName, commandTransition.fadeDuration);
                    return;
                }
            }
        }
    }
}