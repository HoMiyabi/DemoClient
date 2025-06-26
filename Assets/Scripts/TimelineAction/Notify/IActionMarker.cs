using UnityEngine.Playables;

namespace Kirara.TimelineAction
{
    public interface IActionMarker
    {
        public void PlayInTimelineEditor(Playable origin);
    }
}