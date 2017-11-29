using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tars.Csharp.Codecs;

namespace Tars.Csharp.Rpc.Clients
{
    public abstract class RpcClient : IRpcClient
    {
        protected IDictionary<Type, RpcMetadata> metadatas;

        public RpcClient(IDictionary<Type, RpcMetadata> metadatas)
        {
            this.metadatas = metadatas;
        }

        public object Inovke(RpcContext context, string methodName, object[] parameters)
        {
            if (!metadatas.TryGetValue(context.InterfaceType, out RpcMetadata metadata)
                || !metadata.Methods.TryGetValue(methodName, out RpcMethodMetadata methodMetadata)) return null;

            var packet = context.CreatePacket();
            packet.FuncName = methodName;
            packet.FuncMetadata = methodMetadata;
            packet.FuncParameters = parameters;
            var request = metadata.Codec.EncodeRequest(packet);
            return SendAsync(context, request);
        }

        public abstract Task ShutdownAsync();

        public abstract object SendAsync(RpcContext context, IByteBuffer request);
    }
}