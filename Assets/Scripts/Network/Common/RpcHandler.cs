using System;
using System.Diagnostics;
using Google.Protobuf;

namespace Kirara.Network
{
    public abstract class RpcHandler<TReq, TRsp> : IMsgHandler
        where TReq : IMessage where TRsp : IMessage, new()
    {
        public void Handle(Session session, IMessage msg, uint rpcSeq)
        {
            var rsp = new TRsp();
            bool isReply = false;

            void Reply()
            {
                if (isReply) return;
                isReply = true;

                if (!KiraraNetwork.MsgMeta.TryGetCmdId(typeof(TRsp), out uint cmdId))
                {
                    MyLog.Error($"消息类型{typeof(TRsp)}没有找到CmdId");
                    return;
                }
                session.Send(cmdId, rpcSeq, rsp);
            }

            try
            {
                Run(session, (TReq)msg, rsp, Reply);
            }
            catch (Exception e)
            {
                MyLog.Error($"消息处理异常: \n{e}\n消息: {msg}");
            }
            finally
            {
                Reply();
            }
        }

        protected abstract void Run(Session session, TReq req, TRsp rsp, Action reply);
    }
}