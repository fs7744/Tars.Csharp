using DotNetty.Transport.Channels;
using System;
using System.Threading.Tasks;
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
            if ("tars_ping".Equals(msg.FuncName, StringComparison.OrdinalIgnoreCase))
            {
                response.Buffer = new byte[0];
            }
            else if (metadatas.TryGetValue(msg.ServantName, out RpcMetadata metadata) &&
                metadata.Methods.TryGetValue(msg.FuncName, out RpcMethodMetadata methodMetadata))
            {
                var parameters = metadata.Codec.DecodeMethodParameters(msg, methodMetadata);
                var returnValue = methodMetadata.Reflector.Invoke(metadata.ServantInstance, parameters);
                if (methodMetadata.Invoke != null)
                {
                    returnValue = methodMetadata.Invoke.StaticInvoke(returnValue);
                }
                response.Buffer = metadata.Codec.EncodeReturnValue(returnValue, parameters, response, methodMetadata);
                //response.Ret = ?
            }
            else
            {
            }

            ctx.WriteAsync(response);
        }
    }
}