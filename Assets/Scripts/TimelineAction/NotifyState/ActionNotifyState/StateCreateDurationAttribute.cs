using System;

namespace Kirara.TimelineAction
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StateCreateDurationAttribute : Attribute
    {
        public float Duration { get; set; }

        public StateCreateDurationAttribute(float duration)
        {
            Duration = duration;
        }
    }
}