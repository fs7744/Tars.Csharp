using DotNetty.Buffers;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Tars.Csharp.Network.Client
{
    public class UdpClient : IUdpClient
    {
        public object SendAsync(EndPoint endPoint, IByteBuffer request, int timeout)
        {
            throw new NotImplementedException();
        }

        public Task ShutdownGracefullyAsync()
        {
            throw new NotImplementedException();
        }
    }
}