using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tars.Csharp.Net.Core
{
    public class IoBuffer
    {
        private MemoryStream ms;
        private BinaryReader br;
        private BinaryWriter wr;

        public IoBuffer(int capacity)
        {
            ms = new MemoryStream(capacity);
            br = new BinaryReader(ms);
            wr = new BinaryWriter(ms);
        }

        public byte Get()
        {
            return br.ReadByte();
        }

        public IoBuffer Get(byte[] dst, int offset, int length)
        {
            ms.Read(dst, offset, length);
            return this;
        }

        public IoBuffer Get(byte[] dst)
        {
            ms.Read(dst, 0, dst.Length);
            return this;
        }

        public short GetShort()
        {
            return br.ReadInt16();
        }

        public int GetInt()
        {
            return br.ReadInt32();
        }

        public IoBuffer Put(byte value)
        {
            return Put(new byte[] { value });
        }

        public IoBuffer Put(byte[] src)
        {
            return Put(src, 0, src.Length);
        }

        public IoBuffer Put(byte[] src, int offset, int length)
        {
            AutoExpand(src.Length, offset, length);
            ms.Write(src, offset, length);
            return this;
        }

        public IoBuffer Put(char c)
        {
            wr.Write(c);
            return this;
        }

        public IoBuffer PutShort(short value)
        {
            wr.Write(value);
            return this;
        }

        public IoBuffer Put(int value)
        {
            wr.Write(value);
            return this;
        }

        public IoBuffer Flip()
        {
            ms.Position = 0;
            return this;
        }

        public int Remaining()
        {
            return (int)(ms.Length - ms.Position);
        }

        public IoBuffer Mark()
        {
            this.buf.mark();
            return this;
        }

        public IoBuffer Reset()
        {
            ms.Position = 0;
            return this;
        }

        public int Position()
        {
            return (int)ms.Position;
        }

        public IoBuffer Position(int newPosition)
        {
            ms.Position = newPosition;
            return this;
        }

        private void AutoExpand(int size, int offset, int length)
        {
            int newCapacity = ms.Capacity;
            int newSize = Position() + length;

            if (size < length) throw new IndexOutOfRangeException();

            while (newSize > newCapacity)
            {
                newCapacity = newCapacity * 2;
            }

            if (newCapacity != ms.Capacity)
            {
                ms.Capacity = newCapacity;
                //if (this.buf.isDirect())
                //{
                //    newBuffer = ByteBuffer.allocateDirect(newCapacity);
                //}
                //else
                //{
                //    newBuffer = ByteBuffer.allocate(newCapacity);
                //}

                //newBuffer.put(this.buf.array());
                //newBuffer.position(this.buf.position());

                //this.buf = newBuffer;
            }
        }
    }
}
