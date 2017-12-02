using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tars.Csharp.Codecs.Attributes;
using Tars.Csharp.Network.Hosting;
using Tars.Csharp.Rpc;

namespace HelloServer
{
    [Rpc(Servant = "TestApp.HelloServer.HelloObj")]
    [TarsCodec]
    public interface IHelloRpc
    {
        string Hello(int no, string name);
    }

    public class HelloServer : IHelloRpc
    {
        public string Hello(int no, string name)
        {
            string result = null;
            if (name.Trim().ToLower() == "Victor")
            {
                result = $"{no}: Sorry, {name}";
            }
            else
            {
                result = $"{no}: Hi, {name}";
            }

            Console.WriteLine(result);
            return result;
        }
    }

    public class Program
    {
        private static void Main(string[] args)
        {
            var kv = new Dictionary<string, string>()
            {
                { ServerHostOptions.Port, "8989" }
            };

            new ServerHostBuilder()
                .ConfigureAppConfiguration(i => i.AddInMemoryCollection(kv))
                .ConfigureServices(i => i.AddLogging(j => j.AddConsole().SetMinimumLevel(LogLevel.Trace)))
                .UseRpc(RpcMode.Udp, true, typeof(Program).Assembly)
                .Build()
                .Run();
        }
    }
}