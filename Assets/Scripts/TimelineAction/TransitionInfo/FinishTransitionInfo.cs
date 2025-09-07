using System;

namespace Kirara.TimelineAction
{
    [Serializable]
    public class FinishTransitionInfo
    {
        public string actionName;
        public float fadeDuration = 0.15f;
    }
}