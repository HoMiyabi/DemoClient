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

        public virtual void NotifyBegin(ActionPlayer player) {}
        public virtual void NotifyTick(ActionPlayer player, float time) {}
        public virtual void NotifyEnd(ActionPlayer player) {}
        public ClipCaps clipCaps => ClipCaps.None;
    }
}