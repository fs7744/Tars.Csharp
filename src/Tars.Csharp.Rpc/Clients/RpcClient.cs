using DotNetty.Buffers;
using System.Threading.Tasks;
using Tars.Csharp.Codecs;
using Tars.Csharp.Network.Client;

namespace Tars.Csharp.Rpc.Clients
{
    public class RpcClient<T> : IRpcClient where T : IClient
    {
        protected RpcClientMetadata metadatas;
        protected T client;

        public RpcClient(RpcClientMetadata metadatas, T client)
        {
            this.metadatas = metadatas;
            this.client = client;
        }

        public object Inovke(RpcContext context, string methodName, object[] parameters)
        {
            if (!metadatas.TryGetValue(context.Servant, out RpcMetadata metadata)
                || !metadata.Methods.TryGetValue(methodName, out RpcMethodMetadata methodMetadata)) return null;

            var packet = context.CreatePacket();
            packet.FuncName = methodName;
            packet.Buffer = metadata.Codec.EncodeMethodParameters(parameters, methodMetadata);
            var request = metadata.Codec.EncodeRequest(packet);
            return SendAsync(context, request);
        }

        public Task ShutdownAsync()
        {
            return client.ShutdownGracefullyAsync();
        }

        public object SendAsync(RpcContext context, IByteBuffer request)
        {
            return client.SendAsync(context.EndPoint, request, context.Timeout);
        }
    }
}