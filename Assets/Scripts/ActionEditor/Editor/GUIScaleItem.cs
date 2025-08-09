using UnityEditor;
using UnityEngine;

namespace ActionEditor.Editor
{
    public static class GUIScaleItem
    {
        public static bool Draw(Rect rect, int num, bool big)
        {
            if (big)
            {
                float lineHeight = rect.height * 0.6f;
                var scaleLineRect = new Rect(rect.x, rect.height - lineHeight, 1f, lineHeight);
                EditorGUI.DrawRect(scaleLineRect, Color.black);
            }
            else
            {
                float lineHeight = rect.height * 0.2f;
                var scaleLineRect = new Rect(rect.x, rect.height - lineHeight, 1f, lineHeight);
                EditorGUI.DrawRect(scaleLineRect, Color.gray);
            }

            var e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && rect.Contains(e.mousePosition))
            {
                e.Use();
                // int frame = NormalizeInt((e.mousePosition - tracksRect.position).x / frameWidth);
                // minFrame = Mathf.Clamp(frame, 0, 100);
                return true;
            }
            return false;
        }
    }
}