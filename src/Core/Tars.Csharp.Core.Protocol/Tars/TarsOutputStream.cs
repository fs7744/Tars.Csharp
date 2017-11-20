/**
 * Tencent is pleased to support the open source community by making Tars available.
 *
 * Copyright (C) 2016THL A29 Limited, a Tencent company. All rights reserved.
 *
 * Licensed under the BSD 3-Clause License (the "License"); you may not use this file except 
 * in compliance with the License. You may obtain a copy of the License at
 *
 * https://opensource.org/licenses/BSD-3-Clause
 *
 * Unless required by applicable law or agreed to in writing, software distributed 
 * under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
 * CONDITIONS OF ANY KIND, either express or implied. See the License for the 
 * specific language governing permissions and limitations under the License.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tars.Csharp.Core.Protocol.Tars.Exceptions;
using Tars.Csharp.Core.Protocol.Util;

namespace Tars.Csharp.Core.Protocol
{
    public class TarsOutputStream
    {
        private MemoryStream ms;
        private BinaryWriter bw;

        public TarsOutputStream(MemoryStream ms)
        {
            this.ms = ms;
            bw = new BinaryWriter(ms, Encoding.BigEndianUnicode);
        }

        public TarsOutputStream(int capacity)
        {
            ms = new MemoryStream(capacity);
            bw = new BinaryWriter(ms, Encoding.BigEndianUnicode);
        }

        public TarsOutputStream()
        {
            ms = new MemoryStream(128);
            bw = new BinaryWriter(ms, Encoding.BigEndianUnicode);
        }

        public MemoryStream GetMemoryStream()
        {
            return ms;
        }

        public Byte[] ToByteArray()
        {
            Byte[] newBytes = new Byte[ms.Position];
            System.Array.Copy(ms.GetBuffer(), 0, newBytes, 0, (int)ms.Position);
            return newBytes;
        }

        public void Reserve(int len)
        {
            int nRemain = ms.Capacity - (int)ms.Length;
            if (nRemain < len)
            {
                ms.Capacity = (ms.Capacity + len) << 1;// -nRemain;
            }
        }

        // 写入头信息
        public void WriteHead(Byte type, int tag)
        {
            if (tag < 15)
            {
                Byte b = (Byte)((tag << 4) | type);

                try
                {
                    {
                        bw.Write(b);
                    }
                }
                catch (Exception e)
                {
                    QTrace.Trace(e.Message);
                }

            }
            else if (tag < 256)
            {
                try
                {
                    Byte b = (Byte)((15 << 4) | type);
                    {
                        bw.Write(b);
                        bw.Write((Byte)tag);
                    }
                }
                catch (Exception e)
                {
                    QTrace.Trace(this + " writeHead: " + e.Message);
                }
            }
            else
            {
                throw new TarsEncodeException("tag is too large: " + tag);
            }
        }

        public void Write(bool b, int tag)
        {
            Byte by = (Byte)(b ? 0x01 : 0);
            Write(by, tag);
        }

        public void Write(Byte b, int tag)
        {
            Reserve(3);
            if (b == 0)
            {
                WriteHead((Byte)TarsStructType.ZeroTag, tag);
            }
            else
            {
                WriteHead((Byte)TarsStructType.Byte, tag);
                try
                {
                    {
                        bw.Write(b);
                    }
                }
                catch (Exception e)
                {
                    QTrace.Trace(e.Message);
                }
            }
        }

        public void Write(short n, int tag)
        {
            Reserve(4);
            if (n >= -128 && n <= 127)
            {
                Write((Byte)n, tag);
            }
            else
            {
                WriteHead((Byte)TarsStructType.Short, tag);
                try
                {
                    {
                        bw.Write(ByteConverter.ReverseEndian(n));
                    }
                }
                catch (Exception e)
                {
                    QTrace.Trace(this + " Write: " + e.Message);
                }
            }
        }
        public void Write(ushort n, int tag)
        {
            Write((short)n, tag);
        }

        public void Write(int n, int tag)
        {
            Reserve(6);

            if (n >= short.MinValue && n <= short.MaxValue)
            {
                Write((short)n, tag);
            }
            else
            {
                WriteHead((Byte)TarsStructType.Int, tag);
                try
                {
                    {
                        bw.Write(ByteConverter.ReverseEndian(n));
                    }
                }
                catch (Exception e)
                {
                    QTrace.Trace(e.Message);
                }
            }
        }

        public void Write(uint n, int tag)
        {
            Write((long)n, tag);
        }

        public void Write(ulong n, int tag)
        {
            Write((long)n, tag);
        }

        public void Write(long n, int tag)
        {
            Reserve(10);
            if (n >= int.MinValue && n <= int.MaxValue)
            {
                Write((int)n, tag);
            }
            else
            {
                WriteHead((Byte)TarsStructType.Long, tag);
                try
                {
                    {
                        bw.Write(ByteConverter.ReverseEndian(n));
                    }
                }
                catch (Exception e)
                {
                    QTrace.Trace(e.Message);
                }
            }
        }

        public void Write(float n, int tag)
        {
            Reserve(6);
            WriteHead((Byte)TarsStructType.Float, tag);
            try
            {
                {
                    bw.Write(ByteConverter.ReverseEndian(n));
                }
            }
            catch (Exception e)
            {
                QTrace.Trace(e.Message);
            }
        }

        public void Write(double n, int tag)
        {
            Reserve(10);
            WriteHead((Byte)TarsStructType.Double, tag);
            try
            {
                {
                    bw.Write(ByteConverter.ReverseEndian(n));
                }
            }
            catch (Exception e)
            {
                QTrace.Trace(e.Message);
            }
        }

        public void WriteStringByte(string s, int tag)
        {
            Byte[] by = HexUtil.hexStr2Bytes(s);
            Reserve(10 + by.Length);
            if (by.Length > 255)
            {
                // 长度大于255，为String4类型
                WriteHead((Byte)TarsStructType.String4, tag);
                try
                {
                    {
                        bw.Write(ByteConverter.ReverseEndian(by.Length));
                        bw.Write(by);
                    }
                }
                catch (Exception e)
                {
                    QTrace.Trace(e.Message);
                }
            }
            else
            {
                // 长度小于255，位String1类型
                WriteHead((Byte)TarsStructType.String1, tag);
                try
                {
                    {
                        bw.Write((Byte)by.Length);
                        bw.Write(by);
                    }
                }
                catch (Exception e)
                {
                    QTrace.Trace(e.Message);
                }
            }
        }

        public void WriteByteString(string s, int tag)
        {
            Reserve(10 + s.Length);
            Byte[] by = HexUtil.hexStr2Bytes(s);
            if (by.Length > 255)
            {
                WriteHead((Byte)TarsStructType.String4, tag);
                try
                {
                    {
                        bw.Write(ByteConverter.ReverseEndian(by.Length));
                        bw.Write(by);
                    }
                }
                catch (Exception e)
                {
                    QTrace.Trace(e.Message);
                }
            }
            else
            {
                WriteHead((Byte)TarsStructType.String1, tag);
                try
                {
                    {
                        bw.Write((Byte)by.Length);
                        bw.Write(by);
                    }
                }
                catch (Exception e)
                {
                    QTrace.Trace(e.Message);
                }
            }
        }

        public void Write(string s, int tag, bool IsLocalString = false)
        {
            Byte[] by;
            try
            {
                by = ByteConverter.String2Bytes(s, IsLocalString);
                if (by == null)
                {
                    by = new Byte[0];
                }
            }
            catch (Exception e)
            {
                QTrace.Trace(e.Message);
                return;
            }
            if (by != null)
            {
                Reserve(10 + by.Length);
            }
            if (by != null && by.Length > 255)
            {
                WriteHead((Byte)TarsStructType.String4, tag);
                try
                {
                    {
                        bw.Write(ByteConverter.ReverseEndian(by.Length));
                        bw.Write(by);
                    }
                }
                catch (Exception e)
                {
                    QTrace.Trace(e.Message);
                }
            }
            else
            {
                WriteHead((Byte)TarsStructType.String1, tag);
                try
                {
                    {
                        if (by != null)
                        {
                            bw.Write((Byte)by.Length);
                            bw.Write(by);
                        }
                        else
                        {
                            bw.Write((Byte)0);
                        }
                    }
                }
                catch (Exception e)
                {
                    QTrace.Trace(e.Message);
                }
            }
        }

        public void Write<K, V>(Dictionary<K, V> m, int tag)
        {
            Reserve(8);
            WriteHead((Byte)TarsStructType.Map, tag);

            Write(m == null ? 0 : m.Count, 0);
            if (m != null)
            {
                foreach (KeyValuePair<K, V> en in m)
                {
                    Write(en.Key, 0);
                    Write(en.Value, 1);
                }
            }
        }

        public void Write(IDictionary m, int tag)
        {
            Reserve(8);
            WriteHead((Byte)TarsStructType.Map, tag);
            Write(m == null ? 0 : m.Count, 0);
            if (m != null)
            {
                ICollection keys = m.Keys;
                foreach (object objKey in keys)
                {
                    Write(objKey, 0);
                    Write(m[objKey], 1);
                }
            }
        }

        public void Write(bool[] l, int tag)
        {
            int nLen = 0;
            if (l != null)
            {
                nLen = l.Length;
            }
            Reserve(8);
            WriteHead((Byte)TarsStructType.List, tag);
            Write(nLen, 0);

            if (l != null)
            {
                foreach (bool e in l)
                {
                    Write(e, 0);
                }
            }
        }

        public void Write(Byte[] l, int tag)
        {
            int nLen = 0;
            if (l != null)
            {
                nLen = l.Length;
            }
            Reserve(8 + nLen);
            WriteHead((Byte)TarsStructType.SimpleList, tag);
            WriteHead((Byte)TarsStructType.Byte, 0);
            Write(nLen, 0);

            try
            {
                if (l != null)
                {
                    bw.Write(l);
                }
            }
            catch (Exception e)
            {
                QTrace.Trace(e.Message);
            }
        }

        public void Write(short[] l, int tag)
        {
            int nLen = 0;
            if (l != null)
            {
                nLen = l.Length;
            }
            Reserve(8);
            WriteHead((Byte)TarsStructType.List, tag);
            Write(nLen, 0);
            if (l != null)
            {
                foreach (short e in l)
                {
                    Write(e, 0);
                }
            }
        }

        public void Write(int[] l, int tag)
        {
            int nLen = 0;
            if (l != null)
            {
                nLen = l.Length;
            }
            Reserve(8);
            WriteHead((Byte)TarsStructType.List, tag);
            Write(nLen, 0);
            if (l != null)
            {
                foreach (int e in l)
                {
                    Write(e, 0);
                }
            }
        }

        public void Write(long[] l, int tag)
        {
            int nLen = 0;
            if (l != null)
            {
                nLen = l.Length;
            }
            Reserve(8);
            WriteHead((Byte)TarsStructType.List, tag);
            Write(nLen, 0);

            if (l != null)
            {
                foreach (long e in l)
                {
                    Write(e, 0);
                }
            }
        }

        public void Write(float[] l, int tag)
        {
            int nLen = 0;
            if (l != null)
            {
                nLen = l.Length;
            }
            Reserve(8);
            WriteHead((Byte)TarsStructType.List, tag);
            Write(nLen, 0);
            if (l != null)
            {
                foreach (float e in l)
                {
                    Write(e, 0);
                }
            }
        }

        public void Write(double[] l, int tag)
        {
            int nLen = 0;
            if (l != null)
            {
                nLen = l.Length;
            }
            Reserve(8);
            WriteHead((Byte)TarsStructType.List, tag);
            Write(nLen, 0);

            if (l != null)
            {
                foreach (double e in l)
                {
                    Write(e, 0);
                }
            }
        }

        public void Write<T>(T[] l, int tag)
        {
            object o = l;
            WriteArray((object[])o, tag);
        }

        private void WriteArray(object[] l, int tag)
        {
            int nLen = 0;
            if (l != null)
            {
                nLen = l.Length;
            }
            Reserve(8);
            WriteHead((Byte)TarsStructType.List, tag);
            Write(nLen, 0);

            if (l != null)
            {
                foreach (Object e in l)
                {
                    Write(e, 0);
                }
            }
        }

        // 由于list，应该在第一个位置[0]预置一个元素（可以为空），以方便判断元素类型
        public void WriteList(IList l, int tag)
        {
            Reserve(8);
            WriteHead((Byte)TarsStructType.List, tag);
            Write(l == null ? 0 : (l.Count > 0 ? l.Count : 0), 0);
            if (l != null)
            {
                for (int i = 0; i < l.Count; i++)
                {
                    Write(l[i], 0);
                }
            }
        }

        public void Write(TarsStructBase o, int tag)
        {
            if (o == null)
            {
                return;
            }
            Reserve(2);
            WriteHead((Byte)TarsStructType.StructBegin, tag);
            o.WriteTo(this);
            Reserve(2);
            WriteHead((Byte)TarsStructType.StructEnd, 0);
        }

        public void Write(object o, int tag)
        {
            if (o == null)
            {
                return;
            }
            if (o is Byte)
            {
                Write(((Byte)o), tag);
            }
            else if (o is Boolean)
            {
                Write((bool)o, tag);
            }
            else if (o is short)
            {
                Write(((short)o), tag);
            }
            else if (o is ushort)
            {
                Write(((int)(ushort)o), tag);
            }
            else if (o is int)
            {
                Write(((int)o), tag);
            }
            else if (o is uint)
            {
                Write((long)(uint)o, tag);
            }
            else if (o is long)
            {
                Write(((long)o), tag);
            }
            else if (o is ulong)
            {
                Write(((long)(ulong)o), tag);
            }
            else if (o is float)
            {
                Write(((float)o), tag);
            }
            else if (o is double)
            {
                Write(((double)o), tag);
            }
            else if (o is string)
            {
                string strObj = o as string;
                Write(strObj, tag);
            }
            else if (o is TarsStructBase)
            {
                Write((TarsStructBase)o, tag);
            }
            else if (o is Byte[])
            {
                Write((Byte[])o, tag);
            }
            else if (o is bool[])
            {
                Write((bool[])o, tag);
            }
            else if (o is short[])
            {
                Write((short[])o, tag);
            }
            else if (o is int[])
            {
                Write((int[])o, tag);
            }
            else if (o is long[])
            {
                Write((long[])o, tag);
            }
            else if (o is float[])
            {
                Write((float[])o, tag);
            }
            else if (o is double[])
            {
                Write((double[])o, tag);
            }
            else if (o.GetType().IsArray)
            {
                Write((object)(object[])o, tag);
            }
            else if (o is IList)
            {
                WriteList((IList)o, tag);
            }
            else if (o is IDictionary)
            {
                Write((IDictionary)o, tag);
            }
            else
            {
                throw new TarsEncodeException(
                        "write object error: unsupport type. " + o.ToString() + "\n");
            }
        }

        protected string sServerEncoding = "UTF-8";
        public int SetServerEncoding(string se)
        {
            sServerEncoding = se;
            return 0;
        }
    }
}