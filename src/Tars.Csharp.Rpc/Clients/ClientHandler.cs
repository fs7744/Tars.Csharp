using DotNetty.Transport.Channels;
using System;
using Tars.Csharp.Codecs;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Rpc.Clients
{
    public class ClientHandler : SimpleChannelInboundHandler<RequestPacket>
    {
        public override bool IsSharable => true;

        private RpcClientMetadata metadatas;
        public ClientHandler(RpcClientMetadata metadatas)
        {
            this.metadatas = metadatas;
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
                var info = metadata.Codec.DecodeReturnValue(msg.Buffer, methodMetadata);
                Console.WriteLine(info);
            }
        }
    }
}