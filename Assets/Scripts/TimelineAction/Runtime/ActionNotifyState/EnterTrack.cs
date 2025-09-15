using System.ComponentModel;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [TrackClipType(typeof(ActionNotifyState)), DisplayName("进入轨道")]
    public class EnterTrack : ActionNotifyStateTrack
    {
    }
}