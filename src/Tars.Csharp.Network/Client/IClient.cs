using System;
using System.Threading.Tasks;

namespace Tars.Csharp.Network.Client
{
    public interface IClient
    {
        Task ShutdownGracefullyAsync();
    }
}