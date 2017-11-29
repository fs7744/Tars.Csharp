using DotNetty.Buffers;
using System;

namespace Tars.Csharp.Attributes
{
    public abstract class CodecAttribute : Attribute
    {
        public abstract object DecodeRequest(IByteBuffer input);

        public abstract object DecodeResponse(IByteBuffer input);

        public abstract IByteBuffer EncodeRequest(object request);

        public abstract IByteBuffer EncodeResponse(object response);
    }
}