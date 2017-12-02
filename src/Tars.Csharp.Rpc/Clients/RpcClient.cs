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
            var buf = Unpooled.Buffer(128);
            buf.WriteInt(0);
            metadata.Codec.EncodeRequest(buf, packet);
            var length = buf.WriterIndex;
            buf.SetWriterIndex(0);
            buf.WriteInt(length);
            buf.SetWriterIndex(length);
            var task = client.SendAsync(context.EndPoint, buf);
            var r = callBackHandler.AddCallBack(context.RequestId, context.Timeout).Result;
            var info = metadata.Codec.DecodeReturnValue(r.Buffer, methodMetadata);
            return info;
        }

        public Task ShutdownAsync()
        {
            return client.ShutdownGracefullyAsync();
        }
    }
}