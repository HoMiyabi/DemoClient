using System.ComponentModel;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [TrackClipType(typeof(ParticleControlPlayableAsset)), DisplayName("粒子控制轨道")]
    public class ParticleControlTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
            clip.duration = 1f;
        }
    }
}