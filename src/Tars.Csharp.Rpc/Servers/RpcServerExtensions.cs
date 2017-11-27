using System.Reflection;
using Tars.Csharp.Rpc;

namespace Tars.Csharp.Network.Hosting
{
    public static class RpcServerExtensions
    {
        public static ServerHostBuilder UseRpc(this ServerHostBuilder builder, Assembly assembly, RpcMode tcp)
        {
            return builder;
        }
    }
}