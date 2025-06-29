using System;
using Google.Protobuf;

namespace Kirara.Network
{
    public interface IMsgHandler
    {
        void Handle(Session session, IMessage msg, uint rpcSeq);
    }
}