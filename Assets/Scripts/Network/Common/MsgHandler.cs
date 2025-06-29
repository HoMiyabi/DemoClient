using System;
using Google.Protobuf;

namespace Kirara.Network
{
    public abstract class MsgHandler<TMsg> : IMsgHandler where TMsg : IMessage
    {
        public void Handle(Session session, IMessage msg, uint rpcSeq)
        {
            try
            {
                Run(session, (TMsg)msg);
            }
            catch (Exception e)
            {
                MyLog.Error($"消息处理异常: \n{e}\n消息: {msg}");
            }
        }

        protected abstract void Run(Session session, TMsg msg);
    }
}