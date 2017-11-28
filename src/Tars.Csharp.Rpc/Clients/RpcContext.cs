using Tars.Csharp.Rpc.DynamicProxy;

namespace Tars.Csharp.Rpc.Clients
{
    public class RpcContext : DynamicProxyContext
    {
        public string Name { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
        public IRpcClient Client { get; set; }
        public RpcMode Mode { get; set; }
    }
}