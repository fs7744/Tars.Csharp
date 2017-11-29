using DotNetty.Buffers;
using System;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Codecs.Attributes
{
    public abstract class CodecAttribute : Attribute
    {
        public abstract string Key { get; }

        public abstract RequestPacket DecodeRequest(IByteBuffer input);

        public abstract RequestPacket DecodeResponse(IByteBuffer input);

        public abstract IByteBuffer EncodeRequest(RequestPacket request);

        public abstract IByteBuffer EncodeResponse(RequestPacket response);
    }
}