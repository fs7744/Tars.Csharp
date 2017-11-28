using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Tars.Csharp.Rpc;
using Tars.Csharp.Rpc.Attributes;
using Tars.Csharp.Rpc.Clients;

namespace HelloClient
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            var service = new ServiceCollection()
                .UseSimpleClient()
                .BuildServiceProvider();

            var factory = service.GetRequiredService<IRpcClientFactory>();
            var context = new RpcContext()
            {
                Name = "TestApp.HelloServer.HelloObj",
                Host = "127.0.0.1",
                Port = 8989,
                Mode = RpcMode.Tcp
            };
            var proxy = factory.CreateProxy<IHelloRpc>(context);
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