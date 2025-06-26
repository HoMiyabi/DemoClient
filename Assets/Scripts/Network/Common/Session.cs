using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Kirara.Network
{
    public partial class Session
    {
        public event Action OnDisconnected;
        public object Data { get; set; }

        public DateTime _lastReceiveTime;

        public readonly Socket _socket;
        private readonly NetMessageWorker messageWorker;

        private readonly MyBuffer buffer = new(1024 * 1024);

        private uint _rpcSeq;

        public bool isClosed;

        public Session(Socket socket, NetMessageWorker messageWorker)
        {
            _socket = socket;
            this.messageWorker = messageWorker;
            _lastReceiveTime = DateTime.UtcNow;
        }

        public void Close()
        {
            isClosed = true;
            MyLog.Debug($"与{_socket.RemoteEndPoint}断开");
            _socket.Close();
            OnDisconnected?.Invoke();
        }

        public void Call(IMessage message, Action<IMessage> callback)
        {
            Call(KiraraNetwork.ProtoMeta.GetMsgId(message.GetType()), message, callback);
        }

        public void Call(uint msgId, IMessage message, Action<IMessage> callback)
        {
            uint seq = ++_rpcSeq;
            messageWorker.rspCallbacks[seq] = callback;
            Send(msgId, seq, message);
        }

        public void Send(IMessage message)
        {
            Send(KiraraNetwork.ProtoMeta.GetMsgId(message.GetType()), message);
        }

        public void Send(uint msgId, IMessage message)
        {
            Send(msgId, 0, message);
        }

        public void Send(uint msgId, uint rpcSeq, IMessage message)
        {
            byte[] sendBytes = MyProtocol.GetSendBytes(msgId, rpcSeq, message);
            // Log.Debug($"发送消息: msgId={msgId}");
            _socket.SendAsync(new ArraySegment<byte>(sendBytes), SocketFlags.None);
        }

        private async Task ReceiveEnsureAsync(int size)
        {
            while (buffer.Count < size)
            {
                int num = await _socket.ReceiveAsync(buffer.WriteSegment, SocketFlags.None);
                buffer.wi += num;
                if (num == 0)
                {
                    throw new Exception("Socket Closed.");
                }
            }
        }

        public async Task ReceiveAsync()
        {
            try
            {
                while (true)
                {
                    await ReceiveEnsureAsync(MyProtocol.HeaderSize);
                    MyProtocol.ReadHeader(buffer, out ushort magicNum, out int len, out uint msgId, out uint rpcSeq);

                    if (magicNum != MyProtocol.MagicNum || len < 0)
                    {
                        MyLog.Debug($"非法包, magicNum: {magicNum}, len: {len}");
                        Close();
                        return;
                    }
                    await ReceiveEnsureAsync(len);

                    var parser = KiraraNetwork.ProtoMeta.GetParser(msgId);
                    if (parser == null)
                    {
                        MyLog.Debug($"未知消息, msgId: {msgId}");
                        Close();
                        return;
                    }
                    var msg = parser.ParseFrom(buffer.Dequeue(len));
                    // Log.Debug($"收到消息 msgId:{msgId}, rpcSeq:{rpcSeq}");
                    _lastReceiveTime = DateTime.UtcNow;

                    messageWorker.Enqueue(this, msgId, rpcSeq, msg, KiraraNetwork.ProtoMeta.IsRsp(msgId));

                    buffer.ClearRead();
                }
            }
            catch (Exception e)
            {
                Close();
            }
        }

        public bool CheckTimeout()
        {
            var now = DateTime.UtcNow;
            var dif = now - _lastReceiveTime;
            long ms = dif.Ticks / TimeSpan.TicksPerMillisecond;
            return ms > KiraraNetwork.SessionTimeoutMs;
        }
    }
}