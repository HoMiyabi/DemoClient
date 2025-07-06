using System.ComponentModel;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [TrackClipType(typeof(CancelWindowPlayableAsset)), DisplayName("取消窗口轨道")]
    public class CancelWindowTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
            clip.duration = 0.5f;
            clip.displayName = "到";
        }
    }
}