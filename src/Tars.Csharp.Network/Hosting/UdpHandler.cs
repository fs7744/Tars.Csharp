using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Tars.Csharp.Network.Hosting
{
    public class UdpHandler : SimpleChannelInboundHandler<DatagramPacket>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket msg)
        {
            ctx.FireChannelRead(msg.Content);
        }
    }
}