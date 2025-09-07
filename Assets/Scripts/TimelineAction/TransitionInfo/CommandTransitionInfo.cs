using System;

namespace Kirara.TimelineAction
{
    [Serializable]
    public class CommandTransitionInfo
    {
        [TimelineActionName]
        public string actionName;
        public float fadeDuration = 0.15f;
        public EActionCommand command;
        public EActionCommandPhase phase;

        public bool Check(EActionCommand command, EActionCommandPhase phase)
        {
            return this.command == command && this.phase == phase;
        }
    }
}