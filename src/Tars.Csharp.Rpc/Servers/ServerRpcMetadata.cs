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
            metadatas = assemblies.ScanRpcMetadatas(true, i =>
            {
                var name = i.GetReflector().GetCustomAttribute<RpcAttribute>().Servant;
                return string.IsNullOrWhiteSpace(name) ? i.FullName : name;
            }, i => i.IsClass && !i.IsGenericType && i.GetReflector().IsDefined<RpcAttribute>());
        }

        public bool TryGetValue(string servantName, out RpcMetadata metadata)
        {
            return metadatas.TryGetValue(servantName, out metadata);
        }
    }
}