using DotNetty.Codecs;
using System;
using System.Collections.Generic;
using System.Text;
using Tars.Csharp.Codecs.Tup;
using DotNetty.Transport.Channels;

namespace Tars.Csharp.Codecs
{
    public class TarsEncoder : MessageToMessageEncoder<ResponsePacket>
    {
        protected override void Encode(IChannelHandlerContext context, ResponsePacket message, List<object> output)
        {
            throw new NotImplementedException();
        }
    }
}
