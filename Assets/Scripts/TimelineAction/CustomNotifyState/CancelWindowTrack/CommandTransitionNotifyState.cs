using System.ComponentModel;
using UnityEngine.Serialization;

namespace Kirara.TimelineAction
{
    [DisplayName("指令转移")]
    public class CommandTransitionNotifyState : ActionNotifyState
    {
        public float inputBufferDuration = 0.1f;
        [FormerlySerializedAs("cancelInfo")] public CommandTransitionInfo commandTransition;
    }
}