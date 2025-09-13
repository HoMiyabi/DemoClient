using System;

namespace Kirara.TimelineAction
{
    [Serializable]
    public class SignalTransitionInfo : TransitionInfo
    {
        // 条件
        public string signalName;

        public bool Check(string signalName)
        {
            return this.signalName == signalName;
        }
    }
}