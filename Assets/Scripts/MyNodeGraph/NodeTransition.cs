using System;

namespace Kirara
{
    public enum CommandType
    {
        Move,
        Attack,
        SpecialAttack,
    }

    [Serializable]
    public class NodeTransition
    {
        public float bufferDuration;
        public float beginTime;
        public float endTime;
        [NodeEnum]
        public CommandType command;
    }
}