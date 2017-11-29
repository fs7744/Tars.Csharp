using DotNetty.Buffers;
using System;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Codecs.Attributes
{
    public class TarsCodecAttribute : CodecAttribute
    {
        public override object DecodeRequest(IByteBuffer input)
        {
            var inputStream = new TarsInputStream(input);
            var result = new RequestPacket();
            result.ReadFrom(inputStream);
            input.MarkReaderIndex();
            return result;
        }

        public override object DecodeResponse(IByteBuffer input)
        {
            throw new NotImplementedException();
        }

        public override IByteBuffer EncodeRequest(object request)
        {
            throw new NotImplementedException();
        }

        public override IByteBuffer EncodeResponse(object response)
        {
            var message = response as RequestPacket;
            if (message == null || message.PacketType == Const.OneWay) return null;
            var buf = Unpooled.Buffer(128);
            var outputStream = new TarsOutputStream(buf);
            message.WriteTo(outputStream);
            return buf.Slice();
        }
    }
}