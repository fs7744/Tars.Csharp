using DotNetty.Transport.Channels;

namespace Tars.Csharp.Rpc.Clients
{
    public class ClientHandler : ChannelHandlerAdapter
    {
        public override bool IsSharable => true;

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            var point = context.Channel.RemoteAddress;
            //client.RemoveChannel(point);
            //client.ConnectAsync(point, retryCount);
            base.ChannelInactive(context);
        }
    }
}