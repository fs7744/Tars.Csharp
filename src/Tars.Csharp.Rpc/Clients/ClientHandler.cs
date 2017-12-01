using DotNetty.Transport.Channels;
using Tars.Csharp.Codecs;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Rpc.Clients
{
    public class ClientHandler : SimpleChannelInboundHandler<RequestPacket>
    {
        public override bool IsSharable => true;

        private RpcClientMetadata metadatas;
        private ICallBackHandler<int> callBackHandler;

        public ClientHandler(RpcClientMetadata metadatas, ICallBackHandler<int> callBackHandler)
        {
            this.metadatas = metadatas;
            this.callBackHandler = callBackHandler;
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            var point = context.Channel.RemoteAddress;
            //client.RemoveChannel(point);
            //client.ConnectAsync(point, retryCount);
            base.ChannelInactive(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, RequestPacket msg)
        {
            if (metadatas.TryGetValue(msg.ServantName, out RpcMetadata metadata) &&
                metadata.Methods.TryGetValue(msg.FuncName, out RpcMethodMetadata methodMetadata))
            {
                var info = "ss";
                //var info = metadata.Codec.DecodeReturnValue(msg.Buffer, methodMetadata);
                callBackHandler.SetResult(msg.RequestId, info);
            }
        }
    }
}