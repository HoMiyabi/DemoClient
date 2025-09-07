using System;

namespace Kirara.TimelineAction
{
    [Serializable]
    public class SignalTransitionInfo
    {
        [TimelineActionName]
        public string actionName;
        public float fadeDuration;
        public string signalName;
    }
}