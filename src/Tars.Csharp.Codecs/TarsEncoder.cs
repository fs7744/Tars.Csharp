using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Codecs
{
    public class TarsEncoder : MessageToMessageEncoder<RequestPacket>
    {
        protected override void Encode(IChannelHandlerContext context, RequestPacket message, List<object> output)
        {
            if (message.PacketType == Const.OneWay) return;
            var buf = Unpooled.Buffer(128);
            var outputStream = new TarsOutputStream(buf);
            message.WriteTo(outputStream);
            output.Add(buf);
        }
    }
}