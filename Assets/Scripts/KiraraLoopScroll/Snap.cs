using System;

namespace Kirara.UI
{
    [Serializable]
    public struct Snap
    {
        // 是否启用
        public bool enable;

        // 时长
        public float duration;

        // 启用的速度阈值
        public float speedThreshold;
    }
}