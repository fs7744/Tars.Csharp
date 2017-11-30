using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using Tars.Csharp.Codecs;

namespace Tars.Csharp.Rpc.Clients
{
    public class RpcClientMetadata
    {
        private IDictionary<string, RpcMetadata> metadatas;

        public RpcClientMetadata(Assembly[] assemblies)
        {
            metadatas = assemblies.ScanRpcMetadatas(false, i =>
            {
                var name = i.GetReflector().GetCustomAttribute<RpcAttribute>().Servant;
                return string.IsNullOrWhiteSpace(name) ? i.FullName : name;
            },
                i => i.IsInterface && !i.IsGenericType && i.GetReflector().IsDefined<RpcAttribute>());
        }

        public bool TryGetValue(string servant, out RpcMetadata metadata)
        {
            return metadatas.TryGetValue(servant, out metadata);
        }
    }
}