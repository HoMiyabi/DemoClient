using System;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Kirara.Network
{
    public partial class Session
    {
        public UniTask<T> CallAsync<T>(uint cmdId, IMessage msg) where T : IMessage
        {
            var utcs = new UniTaskCompletionSource<T>();
            try
            {
                Call(cmdId, msg, rsp => CallbackWrapper((T)rsp, utcs).Forget());
            }
            catch (Exception e)
            {
                utcs.TrySetException(e);
            }
            return utcs.Task;
        }

        private async UniTaskVoid CallbackWrapper<T>(T msg, UniTaskCompletionSource<T> utcs) where T : IMessage
        {
            await UniTask.SwitchToMainThread();
            Interceptor(msg, utcs);
            utcs.TrySetResult(msg);
        }

        private void Interceptor<T>(IMessage msg, UniTaskCompletionSource<T> utcs) where T : IMessage
        {
            var fieldDesc = msg.Descriptor.FindFieldByNumber(1);
            if (fieldDesc != null &&
                fieldDesc.FieldType == FieldType.Message &&
                fieldDesc.MessageType == Result.Descriptor)
            {
                var result = (Result)fieldDesc.Accessor.GetValue(msg);
                if (result.Code != 0)
                {
                    utcs.TrySetException(new ResultException(msg, $"[失败] Code: {result.Code}, Msg: {result.Msg}, " +
                                                             $"内容: {msg}"));
                }
            }
        }
    }
}