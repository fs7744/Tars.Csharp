using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tars.Csharp.Network.Client
{
    public class TcpClient : ITcpClient
    {
        public Task ShutdownGracefullyAsync()
        {
            throw new NotImplementedException();
        }
    }
}
