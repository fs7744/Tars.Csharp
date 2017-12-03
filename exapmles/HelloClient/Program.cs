using Hello.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Tars.Csharp.Rpc;
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
            var builder = new ConfigurationBuilder();
            var service = new ServiceCollection()
                .AddSingleton(i => builder.Build())
                .AddLogging(j => j.AddConsole().SetMinimumLevel(LogLevel.Trace))
                .UseSimpleRpcClient(typeof(IHelloRpc).Assembly)
                .BuildServiceProvider();

            var factory = service.GetRequiredService<IRpcClientFactory>();
            var context = new RpcContext()
            {
                Servant = "TestApp.HelloServer.HelloObj",
                EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8989),
                Mode = RpcMode.Udp
            };
            var proxy = factory.CreateProxy<IHelloRpc>(context);
            var result = proxy.Hello(5, "Victor");
            Console.WriteLine(result);
            proxy.HelloHolder(5, out result);
            Console.WriteLine(result);
            Console.ReadKey();
            await factory.ShutdownAsync();
        }
    }
}