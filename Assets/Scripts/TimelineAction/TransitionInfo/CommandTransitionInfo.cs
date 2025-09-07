using System;
using Kirara.TimelineAction;

namespace TimelineAction
{
    [Serializable]
    public class CommandTransitionInfo
    {
        public string actionName;
        public float fadeDuration = 0.15f;
        public EActionCommand command;
        public EActionCommandPhase phase;
    }
}