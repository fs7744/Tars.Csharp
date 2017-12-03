using DotNetty.Buffers;
using System.Linq;
using System.Threading;
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
        private int requestId = 0;

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
            packet.RequestId = Interlocked.Increment(ref requestId);
            packet.FuncName = methodName;
            packet.Buffer = metadata.Codec.EncodeMethodParameters(parameters, packet, methodMetadata);
            var buf = Unpooled.Buffer(128);
            buf.WriteInt(0);
            metadata.Codec.EncodeRequest(buf, packet);
            var length = buf.WriterIndex;
            buf.SetWriterIndex(0);
            buf.WriteInt(length);
            buf.SetWriterIndex(length);
            var task = client.SendAsync(context.EndPoint, buf);
            var r = callBackHandler.AddCallBack(packet.RequestId, context.Timeout).Result;
            var info = metadata.Codec.DecodeReturnValue(r, methodMetadata);
            if (info.Item2 == null) return info.Item1;
            var index = 0;
            foreach (var item in methodMetadata.Parameters.Where(i => i.ParameterType.IsByRef))
            {
                if (index >= info.Item2.Length) break;
                parameters[item.Position] = info.Item2[index++];
            }
            return info.Item1;
        }

        public Task ShutdownAsync()
        {
            return client.ShutdownGracefullyAsync();
        }
    }
}