using UnityEditor;
using UnityEngine;

namespace ActionEditor.Editor
{
    public static class GUIClipItem
    {
        public static void Draw(Rect rect, Color color)
        {
            EditorGUI.DrawRect(rect, color);
        }
    }
}