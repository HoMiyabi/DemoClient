using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    public abstract class ActionNotifyState : PlayableAsset, ITimelineClipAsset
    {
        [NonSerialized] public float start;
        [NonSerialized] public float length;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<ActionNotifyStatePlayable>.Create(graph);
        }

        public virtual void NotifyBegin(ActionCtrl actionCtrl) {}
        public virtual void NotifyTick(ActionCtrl actionCtrl, float time) {}
        public virtual void NotifyEnd(ActionCtrl actionCtrl) {}
        public ClipCaps clipCaps => ClipCaps.None;
    }
}