﻿using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [CustomTimelineEditor(typeof(BoxPlayableAsset))]
    public class BoxPlayableAssetEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = (BoxPlayableAsset)clip.asset;
            clip.displayName = asset.boxType.ToString();
        }
    }
}