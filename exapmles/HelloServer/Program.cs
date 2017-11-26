using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
                .ConfigureServices(i => i.AddLogging(j => j.AddConsole()))
                .UseUdp((i, j) =>
                {
                    var config = i.GetRequiredService<IConfigurationRoot>();
                    var packetMaxSize = config.GetValue(ServerHostOptions.PacketMaxSize, 100 * 1024 * 1024);
                    j.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian, packetMaxSize, 0, 4, -4, 0, true));
                    j.AddLast(new TarsDecoder());
                    j.AddLast(new TestHandler());
                })
                .Build()
                .Run();
        }

    }

    public class TestHandler : SimpleChannelInboundHandler<RequestPacket>
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


        protected override void ChannelRead0(IChannelHandlerContext ctx, RequestPacket msg)
        {
            var ps = test.ReadFrom(msg);
            var ret =  test.Method.Invoke(this, ps) as Task<string>;
            ret.ContinueWith(i => 
            {
                ctx.WriteAndFlushAsync(i.Result);
                Console.WriteLine(i.Result);
            });
        }
    }
}