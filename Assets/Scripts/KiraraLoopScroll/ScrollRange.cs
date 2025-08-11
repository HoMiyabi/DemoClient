using UnityEngine;

namespace Kirara.UI
{
    public struct ScrollRange
    {
        public float min;
        public float max;

        public static readonly ScrollRange Infinity = new(float.NegativeInfinity, float.PositiveInfinity);

        public float Length => max - min;

        public ScrollRange(float min, float max)
        {
            if (min > max)
            {
                Debug.LogWarning($"非法范围, min: {min}, max: {max}");
            }
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// 获取离pos最近的边界
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float GetNearEdge(float pos)
        {
            float len1 = Mathf.Abs(pos - min);
            float len2 = Mathf.Abs(pos - max);
            return len1 < len2 ? min : max;
        }

        /// <summary>
        /// 判断pos是否在范围内
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Contains(float pos)
        {
            return pos >= min && pos <= max;
        }
    }
}