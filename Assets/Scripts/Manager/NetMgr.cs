using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Kirara.Network;
using UnityEngine;

namespace Kirara.Manager
{
    public class NetMgr : UnitySingleton<NetMgr>
    {
        public string host;
        public int port;

        public Session session { get; private set; }

        public void Init()
        {
            KiraraNetwork.Init(new ProtoMeta(), GetType().Assembly);
        }

        public void Connect()
        {
            // session = netScene.Connect(remoteAddress, NetworkProtocolType.KCP,
            //     () =>
            //     {
            //         Debug.Log("连接成功");
            //         session.AddComponent<SessionHeartbeatComponent>().Start(1000);
            //     }, () =>
            //     {
            //         Debug.LogWarning("连接失败");
            //     }, () =>
            //     {
            //         Debug.LogWarning("连接断开");
            //     }, false);
            session = Client.Connect(host, port);
            RepeatSendPing().Forget();
        }

        private const float interval = 1f;
        private readonly WaitForSeconds wait = new(interval);

        private readonly Ping ping = new();
        private double rttMs = 1f;
        private double serverTimeMs;

        private async UniTaskVoid RepeatSendPing()
        {
            while (true)
            {
                await UniTask.WaitForSeconds(interval);

                var t1 = DateTime.UtcNow;
                var msg = await session.CallAsync(ProtoMsgId.Ping, ping);
                var pong = msg as Pong;
                var t2 = DateTime.UtcNow;
                rttMs = (float)(t2 - t1).TotalMilliseconds;
                serverTimeMs = pong.UnixTimeMs + rttMs / 2;
            }
        }

        private void OnApplicationQuit()
        {
            Client.Stop();
            session.Close();
        }
    }
}