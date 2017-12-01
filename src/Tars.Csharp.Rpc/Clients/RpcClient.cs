using System.Threading.Tasks;
using Tars.Csharp.Codecs;
using Tars.Csharp.Network.Client;

namespace Tars.Csharp.Rpc.Clients
{
    public class RpcClient<T> : IRpcClient where T : IClient
    {
        protected RpcClientMetadata metadatas;
        protected T client;
        private ICallBackHandler<int> callBackHandler;

        public RpcClient(RpcClientMetadata metadatas, T client, ICallBackHandler<int> callBackHandler)
        {
            this.metadatas = metadatas;
            this.client = client;
            this.callBackHandler = callBackHandler;
        }

        public object Inovke(RpcContext context, string methodName, object[] parameters)
        {
            if (!metadatas.TryGetValue(context.Servant, out RpcMetadata metadata)
                || !metadata.Methods.TryGetValue(methodName, out RpcMethodMetadata methodMetadata)) return null;

            var packet = context.CreatePacket();
            packet.FuncName = methodName;
            packet.Buffer = metadata.Codec.EncodeMethodParameters(parameters, methodMetadata);
            var request = metadata.Codec.EncodeRequest(packet);
            var task = client.SendAsync(context.EndPoint, request);
            var r = callBackHandler.AddCallBack(context.RequestId, context.Timeout).Result;
            return r;
        }

        public Task ShutdownAsync()
        {
            return client.ShutdownGracefullyAsync();
        }
    }
}