using System;
using System.Collections.Generic;
using System.IO;

namespace Tars.Csharp.Core.Protocol.Util
{
    internal class TarsUtil
    {
        /**
         * Constant to use in building the hashCode.
         */
        private static int iConstant = 37;

        /**
         * Running total of the hashCode.
         */
        private static int iTotal = 17;

        public static bool Equals(bool l, bool r)
        {
            return l == r;
        }

        public static bool Equals(byte l, byte r)
        {
            return l == r;
        }

        public static bool Equals(char l, char r)
        {
            return l == r;
        }

        public static bool Equals(short l, short r)
        {
            return l == r;
        }

        public static bool Equals(int l, int r)
        {
            return l == r;
        }

        public static bool Equals(long l, long r)
        {
            return l == r;
        }

        public static bool Equals(float l, float r)
        {
            return l == r;
        }

        public static bool Equals(double l, double r)
        {
            return l == r;
        }

        public static new bool Equals(object l, object r)
        {
            return l.Equals(r);
        }

        public static int CompareTo(bool l, bool r)
        {
            return (l ? 1 : 0) - (r ? 1 : 0);
        }

        public static int CompareTo(byte l, byte r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int CompareTo(char l, char r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int CompareTo(short l, short r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int CompareTo(int l, int r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int CompareTo(long l, long r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int CompareTo(float l, float r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int CompareTo(double l, double r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int CompareTo<T>(T l, T r) where T : IComparable
        {
            return l.CompareTo(r);
        }

        public static int CompareTo<T>(List<T> l, List<T> r) where T : IComparable
        {
            int n = 0;
            for (int i = 0, j = 0; i < l.Count && j < r.Count; i++, j++)
            {
                if (l[i] is IComparable && r[j] is IComparable)
                {
                    IComparable lc = (IComparable)l[i];
                    IComparable rc = (IComparable)r[j];
                    n = lc.CompareTo(rc);
                    if (n != 0)
                    {
                        return n;
                    }
                }
                else
                {
                    throw new Exception("Argument must be IComparable!");
                }
            }

            return CompareTo(l.Count, r.Count);
        }

        public static int CompareTo<T>(T[] l, T[] r) where T : IComparable
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = l[li].CompareTo(r[ri]);
                if (n != 0)
                    return n;
            }
            return CompareTo(l.Length, r.Length);
        }

        public static int CompareTo(bool[] l, bool[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = CompareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return CompareTo(l.Length, r.Length);
        }

        public static int CompareTo(byte[] l, byte[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = CompareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return CompareTo(l.Length, r.Length);
        }

        public static int CompareTo(char[] l, char[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = CompareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return CompareTo(l.Length, r.Length);
        }

        public static int CompareTo(short[] l, short[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = CompareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return CompareTo(l.Length, r.Length);
        }

        public static int CompareTo(int[] l, int[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = CompareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return CompareTo(l.Length, r.Length);
        }

        public static int CompareTo(long[] l, long[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = CompareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return CompareTo(l.Length, r.Length);
        }

        public static int CompareTo(float[] l, float[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = CompareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return CompareTo(l.Length, r.Length);
        }

        public static int CompareTo(double[] l, double[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = CompareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return CompareTo(l.Length, r.Length);
        }

        public static int HashCode(bool o)
        {
            return iTotal * iConstant + (o ? 0 : 1);
        }

        public static int HashCode(bool[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + (array[i] ? 0 : 1);
                }
                return tempTotal;
            }
        }

        public static int HashCode(byte o)
        {
            return iTotal * iConstant + o;
        }

        public static int HashCode(byte[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + array[i];
                }
                return tempTotal;
            }
        }

        public static int HashCode(char o)
        {
            return iTotal * iConstant + o;
        }

        public static int HashCode(char[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + array[i];
                }
                return tempTotal;
            }
        }

        public static int HashCode(double o)
        {
            return HashCode(Convert.ToInt64(o));
        }

        public static int HashCode(double[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + ((int)(Convert.ToInt64(array[i]) ^ (Convert.ToInt64(array[i]) >> 32)));
                }
                return tempTotal;
            }
        }

        public static int HashCode(float o)
        {
            return iTotal * iConstant + Convert.ToInt32(o);
        }

        public static int HashCode(float[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + Convert.ToInt32(array[i]);
                }
                return tempTotal;
            }
        }

        public static int HashCode(short o)
        {
            return iTotal * iConstant + o;
        }

        public static int HashCode(short[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + array[i];
                }
                return tempTotal;
            }
        }

        public static int HashCode(int o)
        {
            return iTotal * iConstant + o;
        }

        public static int HashCode(int[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + array[i];
                }
                return tempTotal;
            }
        }

        public static int HashCode(long o)
        {
            return iTotal * iConstant + ((int)(o ^ (o >> 32)));
        }

        public static int HashCode(long[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + ((int)(array[i] ^ (array[i] >> 32)));
                }
                return tempTotal;
            }
        }

        public static int HashCode(TarsStruct[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + (array[i].GetHashCode());
                }
                return tempTotal;
            }
        }

        public static int HashCode(object obj)
        {
            if (null == obj)
            {
                return iTotal * iConstant;
            }
            else
            {
                if (obj.GetType().IsArray)
                {
                    if (obj is long[])
                    {
                        return HashCode((long[])obj);
                    }
                    else if (obj is int[])
                    {
                        return HashCode((int[])obj);
                    }
                    else if (obj is short[])
                    {
                        return HashCode((short[])obj);
                    }
                    else if (obj is char[])
                    {
                        return HashCode((char[])obj);
                    }
                    else if (obj is byte[])
                    {
                        return HashCode((byte[])obj);
                    }
                    else if (obj is double[])
                    {
                        return HashCode((double[])obj);
                    }
                    else if (obj is float[])
                    {
                        return HashCode((float[])obj);
                    }
                    else if (obj is bool[])
                    {
                        return HashCode((bool[])obj);
                    }
                    else if (obj is TarsStruct[])
                    {
                        return HashCode((TarsStruct[])obj);
                    }
                    else
                    {
                        return HashCode((Object[])obj);
                    }
                }
                else if (obj is TarsStruct)
                {
                    return obj.GetHashCode();
                }
                else
                {
                    return iTotal * iConstant + obj.GetHashCode();
                }
            }
        }

        public static byte[] GetTarsBufArray(MemoryStream ms)
        {
            byte[] bytes = new byte[ms.Position];
            Array.Copy(ms.GetBuffer(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}