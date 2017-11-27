using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tars.Csharp.Network.Client
{
    public interface IClient
    {
        Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan timeout);

    }
}
