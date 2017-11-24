using DotNetty.Buffers;
using DotNetty.Codecs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Tars.Csharp.Network.Hosting;

namespace HelloServer
{
    internal class Program
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
                })
                .Build()
                .Run();
        }
    }
}