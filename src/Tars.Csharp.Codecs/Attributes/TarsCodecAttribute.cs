using DotNetty.Buffers;
using System;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Codecs.Attributes
{
    public class TarsCodecAttribute : CodecAttribute
    {
        public override string Key => Const.TarsCodec;

        public override RequestPacket DecodeRequest(IByteBuffer input)
        {
            var inputStream = new TarsInputStream(input);
            var result = new RequestPacket();
            result.ReadFrom(inputStream);
            input.MarkReaderIndex();
            return result;
        }

        public override RequestPacket DecodeResponse(IByteBuffer input)
        {
            throw new NotImplementedException();
        }

        public override object[] DecodeMethodParameters(byte[] input, RpcMethodMetadata metdata)
        {
            throw new NotImplementedException();
        }

        public override object DecodeReturnValue(byte[] input, RpcMethodMetadata metdata)
        {
            throw new NotImplementedException();
        }

        public override IByteBuffer EncodeRequest(RequestPacket request)
        {
            throw new NotImplementedException();
        }

        public override IByteBuffer EncodeResponse(RequestPacket response)
        {
            var buf = Unpooled.Buffer(128);
            var outputStream = new TarsOutputStream(buf);
            response.WriteTo(outputStream);
            return buf.Slice();
        }

        public override byte[] EncodeMethodParameters(object[] parameters, RpcMethodMetadata metdata)
        {
            throw new NotImplementedException();
        }

        public override byte[] EncodeReturnValue(object returnValue, RpcMethodMetadata metdata)
        {
            throw new NotImplementedException();
        }
    }
}