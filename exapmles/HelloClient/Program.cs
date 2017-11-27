using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Tars.Csharp.Rpc;
using Tars.Csharp.Rpc.Attributes;
using Tars.Csharp.Rpc.Clients;

namespace HelloClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            var service = new ServiceCollection()
                .AddSingleton<IClientFactory, SimpleClientFactory>()
                .BuildServiceProvider();

            var factory = service.GetRequiredService<IClientFactory>();
            var proxy = factory.GetOrCreateProxy<IHelloRpc>("TestApp.HelloServer.HelloObj", RpcMode.Tcp, "127.0.0.1", 8989);
            Console.WriteLine(await proxy.Hello(1, "Victor"));
            await factory.ShutdownAsync();
        }
    }

    [Rpc(Codec = Codec.Tars)]
    public interface IHelloRpc
    {
        Task<string> Hello(int no, string name);
    }
}