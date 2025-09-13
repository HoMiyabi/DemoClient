using System;

namespace Kirara.TimelineAction
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NotifyStateDurationAttribute : Attribute
    {
        public float Duration { get; set; }

        public NotifyStateDurationAttribute(float duration)
        {
            Duration = duration;
        }
    }
}