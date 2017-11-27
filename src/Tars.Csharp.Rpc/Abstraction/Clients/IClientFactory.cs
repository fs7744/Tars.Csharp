using System.Threading.Tasks;

namespace Tars.Csharp.Rpc.Clients
{
    public interface IClientFactory
    {
        T GetOrCreateProxy<T>(string name, RpcMode mode, string host, int port);
        Task ShutdownAsync();
    }
}