using DotNetty.Transport.Channels;

namespace Tars.Csharp.Network.Client
{
    public abstract class NetworkClientInitializer
    {
        public abstract void Init(IChannel channel);
    }
}