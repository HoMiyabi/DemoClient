using Cysharp.Threading.Tasks;
using Google.Protobuf;
using Kirara.Manager;

public static partial class NetFn
{
    public static void Send(IMessage msg)
    {
        NetMgr.Instance.session.Send(msg);
    }

    public static UniTask<T> CallAsync<T>(uint cmdId, IMessage msg) where T : IMessage
    {
        return NetMgr.Instance.session.CallAsync<T>(cmdId, msg);
    }
}