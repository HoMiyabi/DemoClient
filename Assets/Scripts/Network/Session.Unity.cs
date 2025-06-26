using System;
using Cysharp.Threading.Tasks;
using Google.Protobuf;

namespace Kirara.Network
{
    public partial class Session
    {
        public UniTask<IMessage> CallAsync(IMessage message)
        {
            return CallAsync(KiraraNetwork.ProtoMeta.GetMsgId(message.GetType()), message);
        }

        public UniTask<IMessage> CallAsync(uint msgId, IMessage message)
        {
            var utcs = new UniTaskCompletionSource<IMessage>();
            try
            {
                Call(msgId, message, msg => CallbackWrapper(msg, utcs).Forget());
            }
            catch (Exception e)
            {
                utcs.TrySetException(e);
            }
            return utcs.Task;
        }

        private async UniTaskVoid CallbackWrapper(IMessage msg, UniTaskCompletionSource<IMessage> utcs)
        {
            await UniTask.SwitchToMainThread();
            utcs.TrySetResult(msg);
        }
    }
}