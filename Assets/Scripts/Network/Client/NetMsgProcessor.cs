using System;
using System.Collections.Concurrent;
using Google.Protobuf;
using UnityEngine;

namespace Kirara.Network
{
    public class NetMsgProcessor
    {
        private readonly ConcurrentQueue<(Session session, uint cmdId, uint rpcSeq, IMessage msg)> queue = new();

        public void Enqueue(Session session, uint cmdId, uint rpcSeq, IMessage msg)
        {
            queue.Enqueue((session, cmdId, rpcSeq, msg));
        }

        public void Update()
        {
            while (queue.TryDequeue(out var item))
            {
                try
                {
                    ProcessMsg(item.session, item.cmdId, item.rpcSeq, item.msg);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private void ProcessMsg(Session session, uint cmdId, uint rpcSeq, IMessage msg)
        {
            if (KiraraNetwork.MsgMeta.IsRsp(cmdId))
            {
                if (KiraraNetwork.RpcCallbacks.TryRemove(rpcSeq, out var callback))
                {
                    callback?.Invoke(msg);
                }
                else
                {
                    MyLog.Debug($"RPC回调未找到. CmdId: {cmdId}, RpcSeq: {rpcSeq}");
                }
            }
            else
            {
                if (KiraraNetwork.Handlers.TryGetValue(cmdId, out var handler))
                {
                    handler.Handle(session, msg, rpcSeq);
                }
                else
                {
                    MyLog.Debug($"没有处理方法，CmdId: {cmdId}");
                }
            }
        }
    }
}