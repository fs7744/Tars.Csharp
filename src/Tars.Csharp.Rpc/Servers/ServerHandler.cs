using DotNetty.Transport.Channels;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Rpc
{
    public class ServerHandler : SimpleChannelInboundHandler<RequestPacket>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, RequestPacket msg)
        {
            throw new System.NotImplementedException();
        }
    }
}