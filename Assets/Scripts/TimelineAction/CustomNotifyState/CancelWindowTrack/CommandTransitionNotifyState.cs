using System.ComponentModel;
using TimelineAction;
using UnityEngine.Serialization;

namespace Kirara.TimelineAction
{
    [DisplayName("指令转移")]
    public class CommandTransitionNotifyState : ActionNotifyState
    {
        public float inputBufferDuration = 0.1f;
        [FormerlySerializedAs("cancelInfo")] public CommandTransitionInfo commandTransition;

        // public override void NotifyBegin(ActionPlayer player)
        // {
        //     var actionCtrl = player.GetComponent<ActionCtrl>();
        //     if (!actionCtrl)
        //     {
        //         return;
        //     }
        //     actionCtrl.commandTransitionNotifyStates.Add(this);
        // }
        //
        // public override void NotifyEnd(ActionPlayer player)
        // {
        //     var actionCtrl = player.GetComponent<ActionCtrl>();
        //     if (!actionCtrl)
        //     {
        //         return;
        //     }
        //     actionCtrl.commandTransitionNotifyStates.Remove(this);
        // }

        public bool Check(EActionCommand command, EActionCommandPhase phase, float time)
        {
            return commandTransition.command == command &&
                   commandTransition.phase == phase &
                   time >= start &&
                   time < start + length;
        }

        public bool Inside(float time)
        {
            return time >= start &&
                   time < start + length;
        }
    }
}