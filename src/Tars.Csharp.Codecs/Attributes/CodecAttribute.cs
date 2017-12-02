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

        public abstract object[] DecodeMethodParameters(RequestPacket request, RpcMethodMetadata metdata);

        public abstract object DecodeReturnValue(byte[] input, RpcMethodMetadata metdata);

        public abstract void EncodeRequest(IByteBuffer output, RequestPacket request);

        public abstract void EncodeResponse(IByteBuffer output, RequestPacket response);

        public abstract byte[] EncodeMethodParameters(object[] parameters, RpcMethodMetadata metdata);

        public abstract byte[] EncodeReturnValue(object returnValue, RpcMethodMetadata metdata);
    }
}