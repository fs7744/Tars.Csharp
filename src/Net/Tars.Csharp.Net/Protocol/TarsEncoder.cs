using DotNetty.Buffers;
using DotNetty.Codecs;
using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Transport.Channels;

namespace Tars.Csharp.Net.Protocol
{
    public class TarsEncoder : MessageToMessageEncoder<IByteBuffer>
    {
        protected override void Encode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            if (message.ReadableBytes < 4) return;

            var length = message.ReadInt();
        }
    }
}
