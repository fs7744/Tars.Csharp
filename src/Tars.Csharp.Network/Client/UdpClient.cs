using DotNetty.Buffers;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Tars.Csharp.Network.Client
{
    public class UdpClient : IUdpClient
    {
        public Task ShutdownGracefullyAsync()
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(EndPoint endPoint, IByteBuffer request)
        {
            throw new NotImplementedException();
        }
    }
}