using DotNetty.Codecs;
using System;
using System.Net;
using Tars.Csharp.Net;
using Tars.Csharp.Net.Protocol;

namespace HelloServer
{
    class Program
    {
        static void Main(string[] args)
        {
            new ServerBootstrapBudiler()
             .UseLibuvTcp()
             .ConfigChannelPipeline(i =>
             {
                 i.AddLast(new TarsDecoder());
             })
             .Bind(new IPEndPoint(IPAddress.Any, 8080))
             .RuncAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)).Wait();
        }
    }
}
