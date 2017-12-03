using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Codecs.Attributes
{
    public class TarsCodecAttribute : CodecAttribute
    {
        public override string Key => Const.TarsCodec;

        private IDictionary<short, ITupCodecHandler> tups;

        public IDictionary<short, ITupCodecHandler> Tups
        {
            get { return tups; }
            set
            {
                tups = value;
                VersionHandler = value.Values.First();
            }
        }

        protected ITupCodecHandler VersionHandler { get; private set; }

        protected ITupCodecHandler GetTupByVersion(short version)
        {
            return Tups[version];
        }

        public override RequestPacket DecodeRequest(IByteBuffer input)
        {
            var inputStream = new TarsInputStream(input);
            var result = new RequestPacket
            {
                Version = VersionHandler.DecodeVersion(inputStream)
            };
            var handler = GetTupByVersion(result.Version);
            handler.DecodeRequest(inputStream, result);
            input.MarkReaderIndex();
            return result;
        }

        public override RequestPacket DecodeResponse(IByteBuffer input)
        {
            var inputStream = new TarsInputStream(input);
            var result = new RequestPacket
            {
                Version = VersionHandler.DecodeVersion(inputStream)
            };
            var handler = GetTupByVersion(result.Version);
            handler.DecodeResponse(inputStream, result);
            input.MarkReaderIndex();
            return result;
        }

        public override object[] DecodeMethodParameters(RequestPacket input, RpcMethodMetadata metdata)
        {
            var handler = GetTupByVersion(input.Version);
            return handler.DecodeMethodParameters(input.Buffer, metdata);
        }

        public override Tuple<object, object[]> DecodeReturnValue(RequestPacket input, RpcMethodMetadata metdata)
        {
            var handler = GetTupByVersion(input.Version);
            return handler.DecodeReturnValue(input.Buffer, metdata);
        }

        public override void EncodeRequest(IByteBuffer output, RequestPacket request)
        {
            var handler = GetTupByVersion(request.Version);
            var outputStream = new TarsOutputStream(output);
            handler.EncodeRequest(outputStream, request);
        }

        public override void EncodeResponse(IByteBuffer output, RequestPacket response)
        {
            var handler = GetTupByVersion(response.Version);
            var outputStream = new TarsOutputStream(output);
            handler.EncodeResponse(outputStream, response);
        }

        public override byte[] EncodeMethodParameters(object[] parameters, RequestPacket request, RpcMethodMetadata metdata)
        {
            var handler = GetTupByVersion(request.Version);
            return handler.EncodeMethodParameters(parameters, metdata);
        }

        public override byte[] EncodeReturnValue(object returnValue, object[] parameters, RequestPacket response, RpcMethodMetadata metdata)
        {
            var handler = GetTupByVersion(response.Version);
            return handler.EncodeReturnValue(returnValue, parameters, metdata);
        }
    }
}