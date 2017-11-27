using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace Tars.Csharp.Network.Client
{
    public class NettyTcpClient
    {
        private ILogger<NettyTcpClient> logger;

        public NettyTcpClient(ILogger<NettyTcpClient> logger)
        {
            this.logger = logger;
        }

        private MultithreadEventLoopGroup group = new MultithreadEventLoopGroup();
        private Bootstrap bootstrap = new Bootstrap();
        private ConcurrentDictionary<EndPoint, IChannel> channels = new ConcurrentDictionary<EndPoint, IChannel>();

        public NettyTcpClient Build()
        {
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new ReConnectHandler(this));
                    pipeline.AddLast(new LengthFieldPrepender(4, true));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian, ushort.MaxValue, 0, 4, -4, 4, true));
                }));

            return this;
        }

        private async Task<IChannel> TryConnectAsync(EndPoint endPoint, int retryCount = -1, int millisecondsDelay = 1000)
        {
            var count = retryCount + 1;
            IChannel channel = null;
            Exception e = null;
            while (retryCount == -1 || count > 0)
            {
                count--;
                try
                {
                    channel = await bootstrap.ConnectAsync(endPoint);
                    e = null;
                    channels.AddOrUpdate(endPoint, channel, (x, y) => channel);
                    break;
                }
                catch (Exception ex)
                {
                    e = ex;
                    logger.LogError(ex.Message, ex);
                    await Task.Delay(millisecondsDelay);
                    continue;
                }
            }
            if (e != null)
            {
                throw e;
            }
            return channel;
        }

        public Task<IChannel> ConnectAsync(EndPoint endPoint, int retryCount = -1, int millisecondsDelay = 1000)
        {
            if (channels.TryGetValue(endPoint, out IChannel channel)) return Task.FromResult(channel);
            else return TryConnectAsync(endPoint, retryCount, millisecondsDelay);
        }

        internal bool RemoveChannel(EndPoint point)
        {
            return channels.TryRemove(point, out IChannel channel);
        }

        public Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan timeout)
        {
            return group.ShutdownGracefullyAsync(quietPeriod, timeout);
        }
    }
}