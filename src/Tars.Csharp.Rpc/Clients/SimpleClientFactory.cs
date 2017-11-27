using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Tars.Csharp.Network.Client;

namespace Tars.Csharp.Rpc.Clients
{
    public class SimpleClientFactory : IClientFactory
    {
        private IServiceProvider provider;
        private IClient tcp;
        private IClient udp;

        public SimpleClientFactory(IServiceProvider provider)
        {
            this.provider = provider;
        }

        private IClient GetClient(RpcMode mode)
        {
            return mode == RpcMode.Tcp
                ? (tcp ?? provider.GetRequiredService<ITcpClient>())
                : (udp ?? provider.GetRequiredService<IUdpClient>());
        }

        public T GetOrCreateProxy<T>(string name, RpcMode mode, string host, int port)
        {
            var client = GetClient(mode);

            return default(T);
        }

        public async Task ShutdownAsync()
        {
            await tcp?.ShutdownGracefullyAsync();
            await udp?.ShutdownGracefullyAsync();
        }
    }
}