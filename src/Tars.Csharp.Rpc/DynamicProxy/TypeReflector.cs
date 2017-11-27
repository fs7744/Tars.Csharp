using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tars.Csharp.Rpc.DynamicProxy
{
    public class TypeReflector
    {
        //public static TypeReflector Create(TypeInfo typeInfo)
        //{
        //    return ReflectorCacheUtils<TypeInfo, TypeReflector>.GetOrAdd(typeInfo, info => new TypeReflector(info));
        //}
    }

    internal static class ReflectorCacheUtils<TMemberInfo, TReflector>
    {
        private readonly static ConcurrentDictionary<TMemberInfo, TReflector> dictionary = new ConcurrentDictionary<TMemberInfo, TReflector>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TReflector GetOrAdd(TMemberInfo key, Func<TMemberInfo, TReflector> factory)
        {
            return dictionary.GetOrAdd(key, k => factory(k));
        }
    }
}
