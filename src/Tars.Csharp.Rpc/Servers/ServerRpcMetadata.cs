using AspectCore.Extensions.Reflection;
using System.Collections.Generic;
using System.Reflection;
using Tars.Csharp.Codecs;

namespace Tars.Csharp.Rpc
{
    public class ServerRpcMetadata
    {
        private IDictionary<string, RpcMetadata> metadatas;

        public ServerRpcMetadata(Assembly[] assemblies)
        {
            metadatas = assemblies.ScanRpcMetadatas(true, i => i.GetReflector().GetCustomAttribute<RpcAttribute>().Servant,
                i => i.IsClass && !i.IsGenericType && i.GetReflector().IsDefined<RpcAttribute>());
        }

        public bool TryGetValue(string servantName, out RpcMetadata metadata)
        {
            return metadatas.TryGetValue(servantName, out metadata);
        }
    }
}