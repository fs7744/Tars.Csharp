using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tars.Csharp.Attributes;
using Tars.Csharp.Codecs;
using Tars.Csharp.Rpc.Attributes;

namespace Tars.Csharp.Rpc
{
    public static class RpcMetadataExtensions
    {
        public static IServiceCollection AddRpcMetadatas(this IServiceCollection service, Assembly assembly)
        {
            var metadatas = new Dictionary<Type, RpcMetadata>();
            foreach (var item in assembly.GetExportedTypes()
                .Where(i => i.IsInterface && !i.IsGenericType && i.GetReflector().IsDefined<RpcAttribute>()))
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
                SetMethodMetaDatas(metadata);
                metadatas.Add(item, metadata);
            }

            return service.AddSingleton<IDictionary<Type, RpcMetadata>>(metadatas);
        }

        private static void SetMethodMetaDatas(RpcMetadata metadata)
        {
            foreach (var method in metadata.InterfaceType.GetMethods(BindingFlags.Public))
            {
                var m = new RpcMethodMetadata()
                {
                    Name = method.Name,
                    ReturnInfo = method.ReturnParameter,
                    Parameters = method.GetParameters()
                };
                metadata.Methods.Add(m.Name, m);
            }
        }
    }
}