using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Tars.Csharp.Network.Client;
using Tars.Csharp.Rpc.DynamicProxy;

namespace Tars.Csharp.Rpc.Clients
{
    public static class RpcClientExtensions
    {
        public static IServiceCollection UseSimpleRpcClient(this IServiceCollection service, params Assembly[] assemblies)
        {
            return service.AddSingleton(new RpcClientMetadata(assemblies))
                .AddSingleton<ITcpClient, NettyTcpClient>()
                .AddSingleton<IUdpClient, UdpClient>()
                .AddSingleton<RpcClient<ITcpClient>, RpcClient<ITcpClient>>()
                .AddSingleton<RpcClient<IUdpClient>, RpcClient<IUdpClient>>()
                .AddTransient<NetworkClientInitializer, RpcClientInitializer>()
                .AddSingleton<IRpcClientFactory, SimpleRpcClientFactory>()
                .AddSingleton<IDynamicProxyGenerator, DynamicProxyGenerator>()
                .AddTransient<NetworkClientInitializer, RpcClientInitializer>()
                .AddSingleton<ClientHandler, ClientHandler>();
        }
    }
}