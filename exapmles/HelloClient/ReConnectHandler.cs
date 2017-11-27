using DotNetty.Transport.Channels;

namespace Tars.Csharp.Network.Client
{
    public class ReConnectHandler : ChannelHandlerAdapter
    {
        private int retryCount;
        private NettyTcpClient client;

        public ReConnectHandler(NettyTcpClient client, int retryCount = 3)
        {
            this.retryCount = retryCount;
            this.client = client;
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            var point = context.Channel.RemoteAddress;
            client.RemoveChannel(point);
            //client.ConnectAsync(point, retryCount);
            base.ChannelInactive(context);
        }
    }
}