using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Google.Protobuf;

namespace Kirara.Network
{
    public class NetMessageWorker
    {
        private static Dictionary<uint, IMsgHandler> handlers;
        public readonly Dictionary<uint, Action<IMessage>> rspCallbacks = new();

        private readonly ConcurrentQueue<(Session session, uint msgId, uint rpcSeq, IMessage msg, bool isRsp)> queue = new();
        private bool isWorking;

        public static void Scan(Assembly assembly)
        {
            handlers = new Dictionary<uint, IMsgHandler>();

            var iMsgHandlerType = typeof(IMsgHandler);

            foreach (var type in assembly.GetTypes())
            {
                if (iMsgHandlerType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    var obj = (IMsgHandler)Activator.CreateInstance(type);
                    if (obj == null)
                    {
                        throw new Exception($"{type.FullName}不能实例化");
                    }
                    var msgType = obj.MsgType;

                    handlers.Add(KiraraNetwork.ProtoMeta.GetMsgId(msgType), obj);
                }
            }
        }

        public void Start()
        {
            if (isWorking) return;

            isWorking = true;
            var thread = new Thread(Work)
            {
                Name = "NetMessageWorker"
            };
            thread.Start();
        }

        public void Stop()
        {
            isWorking = false;
        }

        public void Enqueue(Session session, uint msgId, uint rpcSeq, IMessage msg, bool isRsp)
        {
            queue.Enqueue((session, msgId, rpcSeq, msg, isRsp));
        }

        private void Work()
        {
            while (isWorking)
            {
                if (queue.TryDequeue(out var item))
                {
                    (var session, uint msgId, uint rpcSeq, var msg, bool isRsp) = item;
                    if (isRsp)
                    {
                        if (rspCallbacks.Remove(rpcSeq, out var callback))
                        {
                            callback?.Invoke(msg);
                        }
                        else
                        {
                            MyLog.Debug($"RPC Callback not found. msgId:{msgId}, rpcSeq:{rpcSeq}");
                        }
                    }
                    else
                    {
                        if (handlers.TryGetValue(msgId, out var handler))
                        {
                            handler.Handle(session, msg, rpcSeq);
                        }
                        else
                        {
                            MyLog.Debug($"没有处理方法，msgId:{msgId}");
                        }
                    }
                }
            }
        }
    }
}