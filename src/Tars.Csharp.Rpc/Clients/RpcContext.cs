using System;
using System.Collections.Generic;
using System.Net;
using Tars.Csharp.Codecs.Tup;
using Tars.Csharp.Rpc.DynamicProxy;

namespace Tars.Csharp.Rpc.Clients
{
    public class RpcContext : DynamicProxyContext
    {
        public string Name { get; set; }
        public EndPoint EndPoint { get; set; }
        public IRpcClient Client { get; set; }
        public RpcMode Mode { get; set; }
        public Type InterfaceType { get; set; }
        public int Timeout { get; set; }
        public IDictionary<string, string> Context { get; set; }
        public IDictionary<string, string> Status { get; set; }
        public short Version { get; set; }
        public int RequestId { get; set; }
        public int MessageType { get; set; }
        public byte PacketType { get; set; }

        public RequestPacket CreatePacket()
        {
            return new RequestPacket()
            {
                ServantName = Name,
                Version = Version,
                RequestId = RequestId,
                MessageType = MessageType,
                PacketType = PacketType,
                Timeout = Timeout,
                Context = Context,
                Status = Status
            };
        }
    }
}