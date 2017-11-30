using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tars.Csharp.Codecs;
using Tars.Csharp.Codecs.Attributes;

namespace Tars.Csharp.Rpc
{
    public static class RpcMetadataExtensions
    {
        public static IDictionary<T, RpcMetadata> ScanRpcMetadatas<T>(this Assembly[] assembly, bool isGenerateReflector, Func<Type, T> getKey, Func<Type, bool> predicate)
        {
            var metadatas = new Dictionary<T, RpcMetadata>();

            foreach (var item in assembly.SelectMany(i => i.GetExportedTypes().Where(predicate)).Distinct())
            {
                var reflector = item.GetReflector();
                if (!reflector.IsDefined<CodecAttribute>())
                {
                    throw new Exception($"{item.FullName} no CodecAttribute.");
                }

                var metadata = new RpcMetadata()
                {
                    InterfaceType = item,
                    Codec = reflector.GetCustomAttribute<CodecAttribute>(),
                    Methods = new Dictionary<string, RpcMethodMetadata>(StringComparer.OrdinalIgnoreCase)
                };
                SetMethodMetaDatas(metadata, isGenerateReflector);
                metadatas.Add(getKey(item), metadata);
            }

            return metadatas;
        }

        private static void SetMethodMetaDatas(RpcMetadata metadata, bool isGenerateReflector)
        {
            foreach (var method in metadata.InterfaceType.GetMethods(BindingFlags.Public))
            {
                var m = new RpcMethodMetadata()
                {
                    Name = method.Name,
                    ReturnInfo = method.ReturnParameter,
                    Parameters = method.GetParameters(),
                    MethodInfo = method,
                    Reflector = isGenerateReflector ? method.GetReflector() : null
                };
                metadata.Methods.Add(m.Name, m);
            }
        }
    }
}