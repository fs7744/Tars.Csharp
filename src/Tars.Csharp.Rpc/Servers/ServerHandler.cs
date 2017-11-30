using DotNetty.Transport.Channels;
using Tars.Csharp.Codecs;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Rpc
{
    public class ServerHandler : SimpleChannelInboundHandler<RequestPacket>
    {
        public override bool IsSharable => true;

        private ServerRpcMetadata metadatas;
        public ServerHandler(ServerRpcMetadata metadatas)
        {
            this.metadatas = metadatas;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, RequestPacket msg)
        {
            var response = msg.CreateResponse();
            if (metadatas.TryGetValue(msg.ServantName, out RpcMetadata metadata) &&
                metadata.Methods.TryGetValue(msg.FuncName, out RpcMethodMetadata methodMetadata))
            {
                var parameters = metadata.Codec.DecodeMethodParameters(msg, methodMetadata);
                var returnValue = methodMetadata.Reflector.Invoke(metadata.ServantInstance, parameters);
                response.Buffer = metadata.Codec.EncodeReturnValue(returnValue, methodMetadata);
                //response.Ret = ?
                ctx.WriteAsync(msg);
            }
            else
            {
                
            }
        }
    }
}