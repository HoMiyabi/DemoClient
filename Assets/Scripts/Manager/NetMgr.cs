using System;
using System.Net.Sockets;
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

        private readonly NetMsgProcessor processor = new();

        public void Init()
        {
            KiraraNetwork.Init(new MsgMeta().Init(), GetType().Assembly);
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
        }

        public delegate void OnConnectionFailedDel(string message, Action retry);

        public async UniTask ConnectAsync(OnConnectionFailedDel onConnectionFailed)
        {
            Socket socket;
            while (true)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    await socket.ConnectAsync(host, port);
                    break;
                }
                catch (SocketException e)
                {
                    socket.Dispose();
                    await UniTask.SwitchToMainThread();
                    var utcs = new UniTaskCompletionSource();
                    onConnectionFailed?.Invoke(e.Message, () => utcs.TrySetResult());
                    await utcs.Task;
                }
            }

            session = new Session(socket, processor);
            _ = session.ReceiveAsync();
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
                var pong = await session.CallAsync<Pong>(MsgCmdId.Ping, ping);
                var t2 = DateTime.UtcNow;
                rttMs = (float)(t2 - t1).TotalMilliseconds;
                serverTimeMs = pong.UnixTimeMs + rttMs / 2;
            }
        }

        private void OnApplicationQuit()
        {
            session.Close();
        }

        private void Update()
        {
            processor.Update();
        }
    }
}