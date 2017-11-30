using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using Tars.Csharp.Codecs;

namespace Tars.Csharp.Rpc
{
    public class ServerRpcMetadata 
    {
        private IDictionary<Type, RpcMetadata> metadatas;

        public ServerRpcMetadata(Assembly[] assemblies)
        {
            metadatas = assemblies.ScanRpcMetadatas(false,
                i => i.IsClass && !i.IsGenericType && i.GetReflector().IsDefined<RpcAttribute>());
        }

        public bool TryGetValue(Type interfaceType, out RpcMetadata metadata)
        {
            return metadatas.TryGetValue(interfaceType, out metadata);
        }
    }
}