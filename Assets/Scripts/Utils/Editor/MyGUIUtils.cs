using UnityEngine;

namespace Kirara
{
    public static class MyGUIUtils
    {
        private static Color color;
        public static void BeginHighlight(GUIStyle style, bool condition)
        {
            color = GUI.backgroundColor;
            if (condition)
            {
                GUI.backgroundColor = Color.cyan + new Color(0,0, 0.2f);
                style.fontStyle = FontStyle.Bold;
            }
        }

        public static void EndHighlight()
        {
            GUI.backgroundColor = color;
        }
    }
}