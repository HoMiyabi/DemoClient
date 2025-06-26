using System;
using Google.Protobuf;

namespace Kirara.Network
{
    public abstract class RpcHandler<TReq, TRsp> : IMsgHandler
        where TReq : IMessage where TRsp : IMessage, new()
    {
        public Type MsgType => typeof(TReq);

        public void Handle(Session session, IMessage msg, uint rpcSeq)
        {
            var rsp = new TRsp();
            bool isReply = false;

            void Reply()
            {
                if (isReply) return;
                isReply = true;

                uint rspId = KiraraNetwork.ProtoMeta.GetMsgId(rsp.GetType());
                session.Send(rspId, rpcSeq, rsp);
            }

            try
            {
                Run(session, (TReq)msg, rsp, Reply);
            }
            catch (Exception e)
            {
                MyLog.Error(e.ToString());
            }
            finally
            {
                Reply();
            }
        }

        protected abstract void Run(Session session, TReq req, TRsp rsp, Action reply);
    }
}