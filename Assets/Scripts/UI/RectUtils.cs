using UnityEngine;

namespace Kirara.UI
{
    public static class RectUtils
    {
        // 返回是否点在相机面的前方
        public static bool WorldToRectLocal(Vector3 worldPos, RectTransform rectTransform, out Vector2 localPos)
        {
            var screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                screenPos,
                null,
                out localPos);
            return screenPos.z > 0;
        }

        public static void SetRectWorldPos(RectTransform rectTransform, Vector3 worldPos, bool autoHide = true)
        {
            if (WorldToRectLocal(worldPos, rectTransform.parent as RectTransform, out var localPos))
            {
                rectTransform.anchoredPosition = localPos;
                if (autoHide)
                {
                    rectTransform.localScale = Vector3.one;
                }
            }
            else
            {
                if (autoHide)
                {
                    rectTransform.localScale = Vector3.zero;
                }
            }
        }

        public static void SetRectWorldPos(RectTransform rectTransform, Transform follow,
            Vector3 localPos, bool autoHide = true)
        {
            SetRectWorldPos(rectTransform, follow.TransformPoint(localPos), autoHide);
        }
    }
}