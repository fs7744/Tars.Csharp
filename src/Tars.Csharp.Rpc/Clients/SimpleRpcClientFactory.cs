using System;
using System.Threading.Tasks;
using Tars.Csharp.Rpc.DynamicProxy;

namespace Tars.Csharp.Rpc.Clients
{
    public class SimpleRpcClientFactory : IRpcClientFactory
    {
        private IServiceProvider provider;
        private IRpcClient client;
        private IDynamicProxyGenerator generator;

        public SimpleRpcClientFactory(IServiceProvider provider, IDynamicProxyGenerator generator, IRpcClient client)
        {
            this.provider = provider;
            this.generator = generator;
            this.client = client;
        }

        //private IRpcClient GetClient(RpcMode mode)
        //{
        //    return mode == RpcMode.Tcp
        //        ? (tcp ?? provider.GetRequiredService<ITcpClient>())
        //        : (udp ?? provider.GetRequiredService<IUdpClient>());
        //}

        public T CreateProxy<T>(RpcContext context)
        {
            context.Invoke = (ctx, methodName, parameters) => client.Inovke((RpcContext)ctx, methodName, parameters);
            return generator.CreateInterfaceProxy<T>(context);
        }

        public async Task ShutdownAsync()
        {
            await client?.ShutdownAsync();
        }
    }
}