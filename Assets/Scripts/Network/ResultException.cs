using System;
using Google.Protobuf;

namespace Kirara.Network
{
    public class ResultException : Exception
    {
        public IMessage Msg { get; private set; }

        public ResultException(IMessage msg, string message) : base(message)
        {
            Msg = msg;
        }
    }
}