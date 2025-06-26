using System;
using Cysharp.Threading.Tasks;
using Google.Protobuf;

namespace Kirara.Network
{
    public abstract class MsgHandler<TMsg> : IMsgHandler where TMsg : IMessage
    {
        public Type MsgType => typeof(TMsg);

        public void Handle(Session session, IMessage msg, uint rpcSeq)
        {
            HandleInternal(session, msg, rpcSeq).Forget();
        }

        private async UniTaskVoid HandleInternal(Session session, IMessage msg, uint rpcSeq)
        {
            await UniTask.SwitchToMainThread();
            Run(session, (TMsg)msg);
        }

        protected abstract void Run(Session session, TMsg msg);
    }
}