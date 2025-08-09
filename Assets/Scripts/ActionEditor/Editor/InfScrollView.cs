using UnityEngine;

namespace ActionEditor.Editor
{
    public static class InfScrollView
    {
        public static Vector2 Begin(Rect rect, Vector2 scrollPos, float scrollStepX = 10f, float scrollStepY = 10f)
        {
            var e = Event.current;
            if (e.type == EventType.ScrollWheel && rect.Contains(e.mousePosition))
            {
                e.Use();
                scrollPos.x -= e.delta.y / Mathf.Abs(e.delta.y) * Mathf.Abs(scrollStepX); // 滚轮向下, dy为正
                scrollPos.x = Mathf.Min(0, scrollPos.x);
                Debug.Log(scrollPos);
            }

            GUI.BeginClip(new Rect(rect.position, Vector2.positiveInfinity),
                scrollPos, Vector2.zero, false);
            return scrollPos;
        }

        public static void End()
        {
            GUI.EndClip();
        }
    }
}