using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [CustomTimelineEditor(typeof(CommandTransitionNotifyState))]
    public class CommandTransitionNotifyStateClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            clip.displayName = "到" + ((CommandTransitionNotifyState)clip.asset).commandTransition.actionName;
        }
    }
}