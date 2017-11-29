using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tars.Csharp.Codecs;
using Tars.Csharp.Network.Client;

namespace Tars.Csharp.Rpc.Clients
{
    public class TcpRpcClient : RpcClient
    {
        private ITcpClient tcp;

        public TcpRpcClient(ITcpClient tcp, IDictionary<Type, RpcMetadata> metadatas) : base(metadatas)
        {
            this.tcp = tcp;
        }

        public override object SendAsync(RpcContext context, IByteBuffer request)
        {
            throw new System.NotImplementedException();
        }

        public override Task ShutdownAsync()
        {
            return tcp.ShutdownGracefullyAsync();
        }
    }
}