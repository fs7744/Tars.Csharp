using DotNetty.Transport.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using Tars.Csharp.Codecs;
using Tars.Csharp.Codecs.Attributes;
using Tars.Csharp.Network.Hosting;

namespace Tars.Csharp.Rpc
{
    public static class RpcServerExtensions
    {
        public static ServerHostBuilder UseRpc(this ServerHostBuilder builder, Assembly assembly, RpcMode mode = RpcMode.Tcp, bool isLibuv = true)
        {
            UseTarsCodec(builder);
            builder.ConfigureServices(i => i.AddSingleton<TarsDecoder, TarsDecoder>()
                                           .AddSingleton<TarsEncoder, TarsEncoder>()
                                           .AddSingleton<ServerHandler, ServerHandler>());
            ConfigHost(builder, mode, isLibuv);
            return builder;
        }

        private static void ConfigHost(ServerHostBuilder builder, RpcMode mode, bool isLibuv)
        {
            Action<IServiceProvider, IChannelPipeline> action = (i, j) =>
            {
                var config = i.GetRequiredService<IConfigurationRoot>();
                var decoder = i.GetRequiredService<TarsDecoder>();
                var encoder = i.GetRequiredService<TarsEncoder>();
                var handler = i.GetRequiredService<ServerHandler>();
                var packetMaxSize = config.GetValue(ServerHostOptions.PacketMaxSize, 100 * 1024 * 1024);
                var lengthFieldLength = config.GetValue(ServerHostOptions.LengthFieldLength, 4);
                j.AddLengthFieldHanlder(packetMaxSize, lengthFieldLength);
                j.AddLast(decoder, encoder, handler);
            };

            switch (mode)
            {
                case RpcMode.Udp:
                    builder.UseUdp(action);
                    break;

                case RpcMode.Tcp when isLibuv:
                    builder.UseLibuvTcp(action);
                    break;

                default:
                    builder.UseTcp(action);
                    break;
            }
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