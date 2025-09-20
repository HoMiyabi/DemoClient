using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [CustomTimelineEditor(typeof(BoxNotifyState))]
    public class BoxNotifyStateClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = (BoxNotifyState)clip.asset;
            clip.displayName = asset.boxType.ToString();
        }
    }
}