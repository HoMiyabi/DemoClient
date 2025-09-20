using System.ComponentModel;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [TrackClipType(typeof(RandomAudioNotifyState)),
     DisplayName("随机音频轨道")]
    public class RandomAudioTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
            clip.duration = 1f;
            clip.displayName = "随机音频";
        }
    }
}