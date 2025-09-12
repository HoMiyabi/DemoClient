using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Google.Protobuf;

namespace Kirara.Network
{
    public static class KiraraNetwork
    {
        public static MsgMeta MsgMeta { get; private set; }
        public static int SessionTimeoutMs { get; private set; } = 8000;
        public static Dictionary<uint, IMsgHandler> Handlers { get; private set; }
        public static ConcurrentDictionary<uint, Action<IMessage>> RpcCallbacks { get; private set; } = new();

        public static void Init(MsgMeta msgMeta, Assembly assembly)
        {
            MsgMeta = msgMeta;
            Scan(assembly);
        }

        private static void Scan(Assembly assembly)
        {
            Handlers = new Dictionary<uint, IMsgHandler>();

            var iMsgHandlerType = typeof(IMsgHandler);

            foreach (var type in assembly.GetTypes())
            {
                if (iMsgHandlerType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    var handler = (IMsgHandler)Activator.CreateInstance(type);
                    if (handler == null)
                    {
                        throw new Exception($"{type.FullName}不能实例化");
                    }
                    if (type.BaseType == null)
                    {
                        throw new Exception($"{type.FullName}没有基类");
                    }
                    var msgType = type.BaseType.GenericTypeArguments[0];
                    if (MsgMeta.TryGetCmdId(msgType, out uint cmdId))
                    {
                        Handlers.Add(cmdId, handler);
                    }
                    else
                    {
                        throw new Exception($"{type.FullName}找不到CmdId");
                    }
                }
            }
        }
    }
}