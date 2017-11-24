using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Tars.Csharp.Network.Hosting
{
    public interface IServerHost
    {
        Task RunAsync();

        void Run();
    }
}