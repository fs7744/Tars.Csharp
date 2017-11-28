using System.Threading.Tasks;

namespace Tars.Csharp.Rpc.Clients
{
    public class SimpleRpcClient : IRpcClient
    {
        public object Inovke(RpcContext context, string methodName, object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public Task ShutdownAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}