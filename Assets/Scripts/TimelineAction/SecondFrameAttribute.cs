using UnityEngine;

namespace Kirara.TimelineAction
{
    public class SecondFrameAttribute : PropertyAttribute
    {
        public float FrameRate { get; set; }

        public SecondFrameAttribute(float frameRate)
        {
            FrameRate = frameRate;
        }
    }
}