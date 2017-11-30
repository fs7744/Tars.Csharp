using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tars.Csharp.Codecs;
using Tars.Csharp.Rpc.Clients;

namespace Tars.Csharp.Rpc
{
    public class ServerRpcMetadata
    {
        internal IDictionary<string, RpcMetadata> metadatas;

        public ServerRpcMetadata(Assembly[] assemblies)
        {
            metadatas = new RpcClientMetadata(assemblies, true).metadatas;
            var ms = metadatas.Values.ToList();
            foreach (var item in assemblies
                .SelectMany(i => i.GetExportedTypes()
                    .Where(j => j.IsClass && !j.IsGenericType && !j.IsAbstract)).Distinct())
            {
                var interfaceMetadata = ms.FirstOrDefault(x => x.InterfaceType.IsAssignableFrom(item));
                if (interfaceMetadata == null) continue;
                ms.Remove(interfaceMetadata);
                interfaceMetadata.ServantType = item;
            }
            foreach (var item in metadatas.Values.Where(i => i.ServantType == null).ToArray())
            {
                metadatas.Remove(item.Servant);
            }
        }

        public bool TryGetValue(string servantName, out RpcMetadata metadata)
        {
            return metadatas.TryGetValue(servantName, out metadata);
        }
    }
}