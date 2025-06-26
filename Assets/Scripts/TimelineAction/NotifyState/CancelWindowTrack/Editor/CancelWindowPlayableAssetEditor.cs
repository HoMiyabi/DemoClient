using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [CustomTimelineEditor(typeof(CancelWindowPlayableAsset))]
    public class CancelWindowPlayableAssetEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            clip.displayName = "到" + ((CancelWindowPlayableAsset)clip.asset).cancelInfo.actionName;
        }
    }
}