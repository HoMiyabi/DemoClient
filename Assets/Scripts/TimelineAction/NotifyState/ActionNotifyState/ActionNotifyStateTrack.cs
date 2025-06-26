using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [TrackClipType(typeof(ActionNotifyState)), DisplayName("动作通知状态轨道")]
    public class ActionNotifyStateTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);

            var state = (ActionNotifyState)clip.asset;
            var type = state.GetType();
            var durationAttr = type.GetCustomAttribute<StateCreateDurationAttribute>();
            if (durationAttr != null)
            {
                clip.duration = durationAttr.Duration;
            }
            else
            {
                clip.duration = 1f;
            }
            var nameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (nameAttr != null)
            {
                clip.displayName = nameAttr.DisplayName;
            }
        }
    }
}