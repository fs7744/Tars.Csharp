using DotNetty.Transport.Channels;
using Tars.Csharp.Codecs;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Rpc
{
    public class ServerHandler : SimpleChannelInboundHandler<RequestPacket>
    {
        private ServerRpcMetadata metadatas;
        public ServerHandler(ServerRpcMetadata metadatas)
        {
            this.metadatas = metadatas;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, RequestPacket msg)
        {
            if (!metadatas.TryGetValue(msg.ServantName, out RpcMetadata metadata) ||
                !metadata.Methods.TryGetValue(msg.FuncName, out RpcMethodMetadata methodMetadata))
            {

            }
            else
            {
                
            }
        }
    }
}