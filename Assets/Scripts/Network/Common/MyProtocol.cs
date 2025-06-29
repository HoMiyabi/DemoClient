using System;
using System.Buffers.Binary;
using Google.Protobuf;

namespace Kirara.Network
{
    public static class MyProtocol
    {
        public const ushort MagicNum = 0x4517;

        public const int MagicNumFiledSize = sizeof(ushort);
        public const int LenFieldSize = sizeof(int);
        public const int CmdIdFieldSize = sizeof(uint);
        public const int RpcSeqFieldSize = sizeof(uint);
        public const int HeaderSize = MagicNumFiledSize + LenFieldSize + CmdIdFieldSize + RpcSeqFieldSize;

        public static void ReadHeader(MyBuffer buffer,
            out ushort magicNum, out int len, out uint cmdId, out uint rpcSeq)
        {
            magicNum = BinaryPrimitives.ReadUInt16BigEndian(buffer.Dequeue(MagicNumFiledSize));
            len = BinaryPrimitives.ReadInt32BigEndian(buffer.Dequeue(LenFieldSize));
            cmdId = BinaryPrimitives.ReadUInt32BigEndian(buffer.Dequeue(CmdIdFieldSize));
            rpcSeq = BinaryPrimitives.ReadUInt32BigEndian(buffer.Dequeue(RpcSeqFieldSize));
        }

        private static void WriteHeader(MyBuffer buffer, int len, uint cmdId, uint rpcSeq)
        {
            BinaryPrimitives.WriteUInt16BigEndian(buffer.Enqueue(MagicNumFiledSize), MagicNum);
            BinaryPrimitives.WriteInt32BigEndian(buffer.Enqueue(LenFieldSize), len);
            BinaryPrimitives.WriteUInt32BigEndian(buffer.Enqueue(CmdIdFieldSize), cmdId);
            BinaryPrimitives.WriteUInt32BigEndian(buffer.Enqueue(RpcSeqFieldSize), rpcSeq);
        }

        public static byte[] GetSendBytes(uint cmdId, uint rpcSeq, IMessage msg)
        {
            int len = msg.CalculateSize();
            var buffer = new MyBuffer(HeaderSize + len);
            WriteHeader(buffer, len, cmdId, rpcSeq);

            msg.WriteTo(buffer.Enqueue(len));

            return buffer.buffer;
        }
    }
}