using UnityEngine;

namespace Kirara.TimelineAction
{
    public class TimeFieldAttribute : PropertyAttribute
    {
        public float FrameRate { get; set; }

        public TimeFieldAttribute(float frameRate)
        {
            FrameRate = frameRate;
        }
    }
}