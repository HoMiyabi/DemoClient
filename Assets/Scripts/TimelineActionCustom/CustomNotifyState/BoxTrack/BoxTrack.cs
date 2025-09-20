using System.ComponentModel;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [TrackClipType(typeof(BoxNotifyState)), DisplayName("盒子轨道")]
    public class BoxTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
            clip.duration = 0.25f;
            clip.displayName = "盒子";
        }
    }
}