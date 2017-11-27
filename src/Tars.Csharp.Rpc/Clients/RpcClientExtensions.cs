using Microsoft.Extensions.DependencyInjection;

namespace Tars.Csharp.Rpc.Clients
{
    public static class RpcClientExtensions
    {
        public static IServiceCollection UseSimpleClient(this IServiceCollection service)
        {
            return service.AddSingleton<IClientFactory, SimpleClientFactory>();
        }
    }
}