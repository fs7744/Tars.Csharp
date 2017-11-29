using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tars.Csharp.Codecs;
using Tars.Csharp.Network.Client;

namespace Tars.Csharp.Rpc.Clients
{
    public class UdpRpcClient : RpcClient
    {
        private IUdpClient udp;

        public UdpRpcClient(IUdpClient udp, IDictionary<Type, RpcMetadata> metadatas) : base(metadatas)
        {
            this.udp = udp;
        }

        public override object SendAsync(RpcContext context, IByteBuffer request)
        {
            throw new System.NotImplementedException();
        }

        public override Task ShutdownAsync()
        {
            return udp.ShutdownGracefullyAsync();
        }
    }
}