using System;
using UnityEditor;
using UnityEditor.Timeline;

namespace Kirara.TimelineAction
{
    public static class TimelineWindowHelper
    {
        private static readonly Type timelineWindowType = GetTimelineWindowType();
        private static Type GetTimelineWindowType()
        {
            var assembly = typeof(TimelineEditorWindow).Assembly;
            return assembly.GetType("UnityEditor.Timeline.TimelineWindow");
        }

        public static TimelineEditorWindow GetWindow()
        {
            return (TimelineEditorWindow)EditorWindow.GetWindow(timelineWindowType);
        }
    }
}