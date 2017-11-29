using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Tars.Csharp.Codecs.Attributes;
using Tars.Csharp.Network.Client;
using Tars.Csharp.Rpc.Attributes;

namespace Tars.Csharp.Rpc.Clients
{
    public static class RpcClientExtensions
    {
        public static IServiceCollection AddRpcProxy(this IServiceCollection service, Assembly assembly)
        {
            foreach (var item in assembly.GetExportedTypes()
                .Where(i => i.IsInterface && !i.IsGenericType && i.GetReflector().IsDefined<RpcAttribute>()))
            {
                var reflector = item.GetReflector();
                if (!reflector.IsDefined<CodecAttribute>())
                {
                    throw new System.Exception($"{item.FullName} no CodecAttribute.");
                }

               // item.GetMethods(BindingFlags.Public).Select(i=> i.GetReflector().ParameterReflectors[0].Position)
            }

            return service;
        }

        public static IServiceCollection UseSimpleRpcClient(this IServiceCollection service)
        {
            return service
                .AddSingleton<ITcpClient, TcpClient>()
                .AddSingleton<IUdpClient, UdpClient>()
                .AddSingleton<IRpcClient, SimpleRpcClient>()
                .AddSingleton<IRpcClientFactory, SimpleRpcClientFactory>();
        }
    }
}