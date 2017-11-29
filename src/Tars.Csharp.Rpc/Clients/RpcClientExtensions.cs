using Microsoft.Extensions.DependencyInjection;
using Tars.Csharp.Network.Client;

namespace Tars.Csharp.Rpc.Clients
{
    public static class RpcClientExtensions
    {
        public static IServiceCollection UseSimpleRpcClient(this IServiceCollection service)
        {
            return service
                .AddSingleton<ITcpClient, NettyTcpClient>()
                .AddSingleton<IUdpClient, UdpClient>()
                .AddSingleton<TcpRpcClient, TcpRpcClient>()
                .AddSingleton<UdpRpcClient, UdpRpcClient>()
                .AddSingleton<IRpcClientFactory, SimpleRpcClientFactory>();
        }
    }
}