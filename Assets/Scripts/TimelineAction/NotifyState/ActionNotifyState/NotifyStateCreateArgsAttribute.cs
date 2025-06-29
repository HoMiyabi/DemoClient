using System;

namespace Kirara.TimelineAction
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NotifyStateCreateArgsAttribute : Attribute
    {
        public float Duration { get; set; }
        public string DisplayName { get; set; }

        public NotifyStateCreateArgsAttribute(float duration, string displayName = null)
        {
            Duration = duration;
            DisplayName = displayName;
        }
    }
}