using System.Threading.Tasks;
using DotNetty.Buffers;
using System.Net;

namespace Tars.Csharp.Network.Client
{
    public interface IClient
    {
        Task ShutdownGracefullyAsync();
        object SendAsync(EndPoint endPoint, IByteBuffer request, int timeout);
    }
}