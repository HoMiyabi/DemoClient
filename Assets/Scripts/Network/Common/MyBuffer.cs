using System;

namespace Kirara.Network
{
    public class MyBuffer
    {
        public readonly byte[] buffer;
        public int wi;
        public int ri;

        public MyBuffer(int size)
        {
            buffer = new byte[size];
        }

        public MyBuffer(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public ArraySegment<byte> WriteSegment => new(buffer, wi, buffer.Length - wi);

        public ReadOnlySpan<byte> ReadSpan => new(buffer, ri, wi - ri);

        public int Count => wi - ri;

        public ReadOnlySpan<byte> Dequeue(int size)
        {
            if (size > wi - ri)
            {
                throw new Exception("buffer not enough");
            }
            var span = new ReadOnlySpan<byte>(buffer, ri, size);
            ri += size;
            return span;
        }

        public Span<byte> Enqueue(int size)
        {
            if (wi + size > buffer.Length)
            {
                throw new Exception("buffer not enough");
            }
            var span = new Span<byte>(buffer, wi, size);
            wi += size;
            return span;
        }

        public void ClearRead()
        {
            Array.Copy(buffer, ri,
                buffer, 0, wi - ri);
            wi -= ri;
            ri = 0;
        }
    }
}