using UnityEngine;

namespace Kirara.TimelineAction.Editor
{
    public static class GUIHighlight
    {
        private static Color oldColor;
        private static readonly Color highlightColor = Color.cyan + new Color(0, 0, 0.2f);

        public static void Begin(GUIStyle style, bool condition)
        {
            oldColor = GUI.backgroundColor;
            if (condition)
            {
                GUI.backgroundColor = highlightColor;
                style.fontStyle = FontStyle.Bold;
            }
        }

        public static void End()
        {
            GUI.backgroundColor = oldColor;
        }
    }
}
