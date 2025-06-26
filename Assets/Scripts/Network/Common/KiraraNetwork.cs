using System.Reflection;

namespace Kirara.Network
{
    public static class KiraraNetwork
    {
        public static IProtoMeta ProtoMeta { get; private set; }
        public static int SessionTimeoutMs { get; private set; } = 8000;

        public static void Init(IProtoMeta protoMeta, Assembly assembly)
        {
            ProtoMeta = protoMeta;
            NetMessageWorker.Scan(assembly);
        }
    }
}