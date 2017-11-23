using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace Tars.Csharp.Net.Protocol
{
    public enum TarResPacket
    {
        Header,
        Body
    }

    public class TarsDecoder : ReplayingDecoder<TarResPacket>
    {
        public TarsDecoder() : base(TarResPacket.Header)
        {
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            switch (State)
            {
                case TarResPacket.Header:
                    input.ReadInt();
                    Checkpoint(TarResPacket.Body);
                    break;

                case TarResPacket.Body:
                    var inputStream = new TarsInputStream(input);
                    var result = new RequestPacket();
                    result.ReadFrom(inputStream);
                    output.Add(result);
                    Checkpoint(TarResPacket.Header);
                    break;

                default:
                    break;
            }
        }
    }
}