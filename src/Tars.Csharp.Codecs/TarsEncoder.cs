using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using Tars.Csharp.Codecs.Attributes;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Codecs
{
    public class TarsEncoder : MessageToMessageEncoder<RequestPacket>
    {
        private TarsCodecAttribute tars;

        public TarsEncoder(TarsCodecAttribute tars)
        {
            this.tars = tars;
        }

        protected override void Encode(IChannelHandlerContext context, RequestPacket message, List<object> output)
        {
            if (message == null || message.PacketType == Const.OneWay) return;
            var res = tars.EncodeResponse(message);
            if (res != null) output.Add(res);
        }
    }
}