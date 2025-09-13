using System;

namespace Kirara.TimelineAction
{
    [Serializable]
    public class TransitionInfo
    {
        [TimelineActionName]
        public string actionName;
        public float fadeDuration = 0.15f;
    }
}