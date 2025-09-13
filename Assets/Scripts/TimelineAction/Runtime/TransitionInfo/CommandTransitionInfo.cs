using System;

namespace Kirara.TimelineAction
{
    [Serializable]
    public class CommandTransitionInfo : TransitionInfo
    {
        // 条件
        public EActionCommand command;
        public EActionCommandPhase phase;

        public bool Check(EActionCommand command, EActionCommandPhase phase)
        {
            return this.command == command && this.phase == phase;
        }
    }
}