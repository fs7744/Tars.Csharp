using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Codecs
{
    //public class TarsDecoder : ReplayingDecoder<TarPacket>
    //{
    //    public TarsDecoder() : base(TarPacket.Header)
    //    {
    //    }

    //    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    //    {
    //        //if (input.ReadableBytes < 1) return;

    //        switch (State)
    //        {
    //            case TarPacket.Header:
    //                Checkpoint(TarPacket.Body);
    //                break;

    //            case TarPacket.Body:
    //                var inputStream = new TarsInputStream(input);
    //                var result = new RequestPacket();
    //                result.ReadFrom(inputStream);
    //                output.Add(result);
    //                Checkpoint(TarPacket.Header);
    //                break;

    //            default:
    //                break;
    //        }
    //    }
    //}

    public class TarsDecoder : ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            var inputStream = new TarsInputStream(input);
            while (input.IsReadable())
            {
                var result = new RequestPacket();
                result.ReadFrom(inputStream);
                output.Add(result);
                input.MarkReaderIndex();
            }
        }
    }
}