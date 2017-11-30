using AspectCore.Extensions.Reflection;
using System.Collections.Generic;
using System.Reflection;
using Tars.Csharp.Codecs;
using Tars.Csharp.Rpc.Clients;
using System.Linq;

namespace Tars.Csharp.Rpc
{
    public class ServerRpcMetadata
    {
        private IDictionary<string, RpcMetadata> metadatas;

        public ServerRpcMetadata(Assembly[] assemblies)
        {
            this.metadatas = new RpcClientMetadata(assemblies).metadatas;
            foreach (var item in metadatas)
            {
               
            }
            metadatas = assemblies.ScanRpcMetadatas(true, i =>
            {
                var name = i.GetReflector().GetCustomAttribute<RpcAttribute>().Servant;
                return string.IsNullOrWhiteSpace(name) ? i.FullName : name;
            }, i => i.IsClass && !i.IsGenericType && !i.IsAbstract 
                && i.GetReflector().IsDefined<RpcAttribute>() 
                && interfaces.Any(j => i.IsAssignableFrom(j.InterfaceType)));
        }

        public bool TryGetValue(string servantName, out RpcMetadata metadata)
        {
            return metadatas.TryGetValue(servantName, out metadata);
        }
    }
}