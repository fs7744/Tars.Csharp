using Hello.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Tars.Csharp.Network.Hosting;
using Tars.Csharp.Rpc;
using Tars.Csharp.Rpc.Clients;

namespace HelloServer
{
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

        public void HelloHolder(int no, out string name)
        {
            name = no.ToString() + "Vic";
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
                .ConfigureServices(i => i.AddLogging(j => j.AddConsole()
                    .SetMinimumLevel(LogLevel.Trace))
                    .UseSimpleRpcClient(typeof(IHelloRpc).Assembly))
                .UseRpc(RpcMode.Udp, true, typeof(IHelloRpc).Assembly, typeof(Program).Assembly)
                .Build()
                .Run();
        }
    }
}