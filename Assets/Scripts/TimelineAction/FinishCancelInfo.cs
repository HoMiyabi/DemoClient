using System;

namespace Kirara.TimelineAction
{
    [Serializable]
    public class FinishCancelInfo
    {
        public string actionName;
        public float fadeDuration = 0.15f;
    }
}