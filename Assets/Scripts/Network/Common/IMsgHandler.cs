using System;
using Google.Protobuf;

namespace Kirara.Network
{
    public interface IMsgHandler
    {
        Type MsgType { get; }
        void Handle(Session session, IMessage msg, uint rpcSeq);
    }
}