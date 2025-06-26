using Google.Protobuf;
using Kirara.Manager;

public static partial class NetFn
{
    public static void Send(IMessage msg)
    {
        NetMgr.Instance.session.Send(msg);
    }
}