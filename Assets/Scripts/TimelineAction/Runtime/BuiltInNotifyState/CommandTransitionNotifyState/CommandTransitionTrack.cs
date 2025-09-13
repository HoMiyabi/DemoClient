using System.ComponentModel;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [TrackClipType(typeof(CommandTransitionNotifyState)),
     DisplayName("指令转移轨道")]
    public class CommandTransitionTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
            clip.duration = 0.5f;
            clip.displayName = "到";
        }
    }
}