using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using Tars.Csharp.Codecs.Attributes;
using Tars.Csharp.Rpc;

namespace Tars.Csharp.Network.Hosting
{
    public static class RpcServerExtensions
    {
        public static ServerHostBuilder UseRpc(this ServerHostBuilder builder, Assembly assembly, RpcMode tcp)
        {
            UseTarsCodec(builder);
            return builder;
        }

        private static void UseTarsCodec(ServerHostBuilder builder)
        {
            builder.ConfigureServices(i =>
            {
                i.AddSingleton<CodecAttribute, TarsCodecAttribute>();
                i.AddSingleton<IDictionary<string, CodecAttribute>>(j =>
                {
                    var dict = new Dictionary<string, CodecAttribute>(StringComparer.OrdinalIgnoreCase);
                    foreach (var item in j.GetServices<CodecAttribute>())
                    {
                        dict.Add(item.Key, item);
                    }
                    return dict;
                });
            });
        }
    }
}