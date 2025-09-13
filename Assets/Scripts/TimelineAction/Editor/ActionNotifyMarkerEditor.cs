using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction.Editor
{
    [CustomTimelineEditor(typeof(ActionNotify))]
    public class ActionNotifyMarkerEditor : MarkerEditor
    {
        public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
        {
            var options = base.GetMarkerOptions(marker);
            if (marker is ActionNotify actionNotify)
            {
                options.tooltip = actionNotify.name;
            }
            return options;
        }
    }
}