//using System.Threading;
//using Tars.Csharp.Net.Protocol;
//using System;
//using DotNetty.Transport.Channels;
//using DotNetty.Transport.Bootstrapping;
//using DotNetty.Transport.Channels.Sockets;
//using DotNetty.Buffers;

//namespace Tars.Csharp.Net.Core
//{
//    public abstract class Server
//    {
//        public const int DefaultMainPort = 9000;

//        //private SelectorManager selectorManager = null;

//        public int MinPoolSize { get; set; } = 10;
//        public int MaxPoolSize { get; set; } = 128;
//        public string Host { get; set; }
//        public int Port { get; set; } = DefaultMainPort;
//        public Processor Processor { get; set; }
//        public IProtocolFactory ProtocolFactory { get; set; }
//        public bool IsKeepAlive { get; set; } = true;
//        public bool IsUdpMode { get; set; }

//        public Server(string host, int port = DefaultMainPort)
//        {
//            Host = host;
//            Port = port;
//        }

//        public void StartUp()
//        {
//            ThreadPool.SetMaxThreads(MinPoolSize, MinPoolSize);
//            ThreadPool.SetMinThreads(MinPoolSize, MinPoolSize);
//            StartNIOServer();

//            //if (!IsUdpMode) SessionManager.getSessionManager().start();
//        }

//        protected void StartNIOServer()
//        {
//            var bossGroup = new MultithreadEventLoopGroup();
//            var workerGroup = new MultithreadEventLoopGroup(4);
//            var bootstrap = new ServerBootstrap();

//            bootstrap
//            .Group(bossGroup, workerGroup)
//            .Channel<TcpServerSocketChannel>()
//            .Option(ChannelOption.SoBacklog, 100)
//            .Option(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
//            .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
//            {
//                var pipeline = channel.Pipeline;
//                pipeline.AddLast(new LengthFieldPrepender(4));
//                pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
//                pipeline.AddLast(new TransportMessageChannelHandlerAdapter(_transportMessageDecoder));
//                pipeline.AddLast(new ServerHandler(async (contenxt, message) =>
//                {
//                    var sender = new DotNettyServerMessageSender(_transportMessageEncoder, contenxt);
//                    await OnReceived(sender, message);
//                }, _logger));
//            }));
//            _channel = await bootstrap.BindAsync(endPoint);

//            //SelectableChannel server = null;
//            int interestKey;

//            //1. Start reactor service
//            //selectorManager.start();

//            //2. Start server on the specified port
//            if (IsUdpMode)
//            {
//                //server = DatagramChannel.open();
//                //((DatagramChannel)server).socket().bind(new InetSocketAddress(host, port));
//                //interestKey = SelectionKey.OP_READ;
//            }
//            else
//            {
//                //server = ServerSocketChannel.open();
//                //((ServerSocketChannel)server).socket().bind(new InetSocketAddress(host, port), 1024);
//                //interestKey = SelectionKey.OP_ACCEPT;
//            }

//            //server.configureBlocking(false);

//            //3. Choose one reactor to handle NIO event
//            //selectorManager.getReactor(0).registerChannel(server, interestKey);
//            Console.WriteLine($"INFO: Server started on port {Port} ...");
//        }
//    }
//}