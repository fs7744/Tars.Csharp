using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tars.Csharp.Codecs;
using Tars.Csharp.Codecs.Tup;
using Tars.Csharp.Network.Hosting;
using Tars.Csharp.Rpc.Protocol;

namespace HelloServer
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var kv = new Dictionary<string, string>()
            {
                { ServerHostOptions.Port, "8989" }
            };

            new ServerHostBuilder()
                .ConfigureAppConfiguration(i => i.AddInMemoryCollection(kv))
                .ConfigureServices(i => i.AddLogging(j => j.AddConsole().SetMinimumLevel(LogLevel.Trace)))
                .UseUdp((i, j) =>
                {
                    var config = i.GetRequiredService<IConfigurationRoot>();
                    var packetMaxSize = config.GetValue(ServerHostOptions.PacketMaxSize, 100 * 1024 * 1024);
                    j.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian, packetMaxSize, 0, 4, -4, 0, true));
                    j.AddLast(new TarsDecoder(), new TarsEncoder());
                    //j.AddLast(new LoggingHandler(i.GetRequiredService<ILogger<LoggingHandler>>()));
                    j.AddLast(new TestHandler());
                })
                .Build()
                .Run();
        }
    }

    public class TestHandler : ChannelHandlerAdapter
    {
        public Task<string> Hello(int no, string name)
        {
            return Task.FromResult($"Hi, {no}, your name is {name}.");
        }

        private TarsMethodInfo test;

        public TestHandler()
        {
            test = new TarsMethodInfo()
            {
                Method = this.GetType().GetMethod("Hello"),
                Parameters = new List<TarsMethodParameterInfo>()
                {
                     new TarsMethodParameterInfo()
                     {
                          Name = "no",
                          Order = 1,
                          Stamp = 0
                     },
                     new TarsMethodParameterInfo()
                     {
                          Name = "name",
                          Order = 2,
                          Stamp = ""
                     }
                }
            };
        }

        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            if (msg is RequestPacket m)
            {
                var ps = test.ReadFrom(m);
                var ret = test.Method.Invoke(this, ps) as Task<string>;
                m.Ret = Const.ServerSuccess;
                IByteBuffer buffer = Unpooled.Buffer(128);
                var s = new TarsOutputStream(buffer);
                s.Write(ret.Result, 0);
                m.Buffer = s.ToByteArray();
                buffer.Release();
                ctx.WriteAndFlushAsync(m);
                Console.WriteLine(ret.Result);
            }
            else
            {
                var ret = test.Method.Invoke(this, new object[] { 33, "die" }) as Task<string>;
                var bytes = Encoding.UTF8.GetBytes(ret.Result);
                IByteBuffer buffer = Unpooled.WrappedBuffer(bytes);
                ctx.WriteAndFlushAsync(buffer);
                Console.WriteLine(ret.Result);
            }
            //ret.ContinueWith(i =>
            //{
            //    ctx.WriteAndFlushAsync(i.Result);
            //    Console.WriteLine(i.Result);
            //});
        }
    }
}