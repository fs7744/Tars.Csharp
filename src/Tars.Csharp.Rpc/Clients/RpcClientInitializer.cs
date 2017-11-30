using DotNetty.Transport.Channels;
using Microsoft.Extensions.Configuration;
using Tars.Csharp.Codecs;
using Tars.Csharp.Codecs.Attributes;
using Tars.Csharp.Network.Client;
using Tars.Csharp.Network.Hosting;

namespace Tars.Csharp.Rpc.Clients
{
    public class RpcClientInitializer : NetworkClientInitializer
    {
        private IConfigurationRoot configuration;
        private ClientHandler handler;

        public RpcClientInitializer(IConfigurationRoot configuration, ClientHandler handler)
        {
            this.configuration = configuration;
            this.handler = handler;
        }

        public override void Init(IChannel channel)
        {
            var packetMaxSize = configuration.GetValue(ServerHostOptions.PacketMaxSize, 100 * 1024 * 1024);
            var lengthFieldLength = configuration.GetValue(ServerHostOptions.LengthFieldLength, 4);
            channel.Pipeline.AddLengthFieldHanlder(packetMaxSize, lengthFieldLength);
            channel.Pipeline.AddLast(new TarsDecoder(new TarsCodecAttribute()), handler);
        }
    }
}