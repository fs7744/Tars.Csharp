using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Tars.Csharp.Rpc.Clients
{
    public static class RpcClientExtensions
    {
        public static IServiceCollection AddRpcProxy(this IServiceCollection service, Assembly assembly)
        {
            return service;
        }

        public static IServiceCollection UseSimpleRpcClient(this IServiceCollection service)
        {
            return service.AddSingleton<IRpcClientFactory, SimpleRpcClientFactory>();
        }
    }
}