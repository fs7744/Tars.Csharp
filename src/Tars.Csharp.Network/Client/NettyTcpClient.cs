using DotNetty.Buffers;
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
    public class NettyTcpClient : ITcpClient
    {
        private ILogger<NettyTcpClient> logger;

        public NettyTcpClient(ILogger<NettyTcpClient> logger, NetworkClientInitializer initializer)
        {
            this.logger = logger;
            Build(initializer);
        }

        private MultithreadEventLoopGroup group = new MultithreadEventLoopGroup();
        private Bootstrap bootstrap = new Bootstrap();
        private ConcurrentDictionary<EndPoint, IChannel> channels = new ConcurrentDictionary<EndPoint, IChannel>();

        public NettyTcpClient Build(NetworkClientInitializer initializer)
        {
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel => initializer.Init(channel)));

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

        public Task ShutdownGracefullyAsync()
        {
            return group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }

        public async Task SendAsync(EndPoint endPoint, IByteBuffer request, int timeout)
        {
            var channel = await ConnectAsync(endPoint);
            await channel.WriteAndFlushAsync(request);
        }
    }
}