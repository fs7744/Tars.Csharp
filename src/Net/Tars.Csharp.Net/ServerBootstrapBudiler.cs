using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Threading.Tasks;

namespace Tars.Csharp.Net
{
    public class ServerBootstrapBudiler
    {
        private Func<IServerChannel> channelFactory;
        private Func<ValueTuple<IEventLoopGroup, IEventLoopGroup>> groupFactory;
        private Action<IChannelPipeline> configChannelPipeline;
        private Dictionary<ChannelOption, ChannelOptionValue> options = new Dictionary<ChannelOption, ChannelOptionValue>();
        private EndPoint address;

        protected abstract class ChannelOptionValue
        {
            public abstract void Set(ServerBootstrap config);
        }

        protected sealed class ChannelOptionValue<T> : ChannelOptionValue
        {
            public ChannelOption<T> Option { get; }
            private readonly T value;

            public ChannelOptionValue(ChannelOption<T> option, T value)
            {
                Option = option;
                this.value = value;
            }

            public override void Set(ServerBootstrap config) => config.Option(this.Option, this.value);
        }

        public ServerBootstrapBudiler UseLibuvTcp(int eventLoopCount = 0)
        {
            channelFactory = () => new TcpServerChannel();
            groupFactory = () =>
            {
                var count = eventLoopCount < 1 ? Environment.ProcessorCount : eventLoopCount;
                var dispatcher = new DispatcherEventLoop();
                var bossGroup = new MultithreadEventLoopGroup(_ => dispatcher, 1);
                var workerGroup = new WorkerEventLoopGroup(dispatcher, count);
                return ValueTuple.Create(bossGroup, workerGroup);
            };
            return this;
        }

        public ServerBootstrapBudiler UseTcp(int eventLoopCount = 0)
        {
            channelFactory = () => new TcpServerSocketChannel();
            groupFactory = () =>
            {
                var count = eventLoopCount < 1 ? Environment.ProcessorCount * 2 : eventLoopCount;
                var bossGroup = new MultithreadEventLoopGroup(1);
                var workerGroup = new MultithreadEventLoopGroup(count);
                return ValueTuple.Create(bossGroup, workerGroup);
            };
            return this;
        }

        public ServerBootstrapBudiler Option<T>(ChannelOption<T> option, T value)
        {
            Contract.Requires(option != null);

            if (value == null)
            {
                options.Remove(option);
            }
            else
            {
                options[option] = new ChannelOptionValue<T>(option, value);
            }
            return this;
        }

        public ServerBootstrapBudiler Bind(EndPoint address)
        {
            this.address = address;
            return this;
        }

        public ServerBootstrapBudiler ConfigChannelPipeline(Action<IChannelPipeline> configChannelPipeline)
        {
            this.configChannelPipeline = configChannelPipeline;
            return this;
        }

        public async Task RuncAsync(TimeSpan quietPeriod, TimeSpan timeout)
        {
            Contract.Requires(channelFactory != null);
            Contract.Requires(groupFactory != null);

            var group = groupFactory();
            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap.Group(group.Item1, group.Item2)
                    .ChannelFactory(channelFactory)
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel => 
                    {
                        channel.Pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian, 100 * 1024 * 1024, 0, 4, -4, 0, true));
                        configChannelPipeline?.Invoke(channel.Pipeline);
                    }));
                foreach (var item in options.Values)
                {
                    item.Set(bootstrap);
                }
                bootstrap.LocalAddress(address);
                IChannel boundChannel = await bootstrap.BindAsync();
                Console.ReadLine();
                await boundChannel.CloseAsync();
            }
            finally
            {
                await Task.WhenAll(
                    group.Item1.ShutdownGracefullyAsync(quietPeriod, timeout),
                    group.Item2.ShutdownGracefullyAsync(quietPeriod, timeout));
            }
        }
    }
}