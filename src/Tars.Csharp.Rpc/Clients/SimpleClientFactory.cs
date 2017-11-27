namespace Tars.Csharp.Rpc.Clients
{
    public class SimpleClientFactory : IClientFactory
    {
        public T GetOrCreateProxy<T>(string name, RpcMode mode, string host, int port)
        {
            throw new System.NotImplementedException();
        }
    }
}