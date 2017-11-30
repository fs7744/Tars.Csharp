using DotNetty.Buffers;
using System;
using System.Collections.Generic;
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

            result.Version = inputStream.Read(result.Version, 1, true);
            result.PacketType = inputStream.Read(result.PacketType, 2, true);
            result.MessageType = inputStream.Read(result.MessageType, 3, true);
            result.RequestId = inputStream.Read(result.RequestId, 4, true);
            result.ServantName = inputStream.ReadString(5, true);
            result.FuncName = inputStream.ReadString(6, true);
            result.Buffer = inputStream.ReadBytes(7, true);
            result.Timeout = inputStream.Read(result.Timeout, 8, true);
            result.Context = inputStream.ReadMap<string, string>(new Dictionary<string, string>(), 9, true);
            result.Status = inputStream.ReadMap<string, string>(new Dictionary<string, string>(), 10, true);
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
            outputStream.Write(response.Version, 1);
            outputStream.Write(response.PacketType, 2);
            switch (response.Version)
            {
                case Const.Version:
                    outputStream.Write(response.RequestId, 3);
                    outputStream.Write(response.MessageType, 4);
                    outputStream.Write(response.Ret, 5);
                    outputStream.Write(response.Buffer, 6);
                    if (response.Status != null && response.Status.Count > 0)
                    {
                        outputStream.Write(response.Status, 7);
                    }
                    if (response.Ret != Const.ServerSuccess)
                    {
                        outputStream.Write(response.ResultDesc ?? "", 8);
                    }
                    break;

                case Const.Version2:
                case Const.Version3:
                    outputStream.Write(response.MessageType, 3);
                    outputStream.Write(response.RequestId, 4);
                    outputStream.Write(response.ServantName, 5);
                    outputStream.Write(response.FuncName, 6);
                    outputStream.Write(response.Buffer, 7);
                    outputStream.Write(response.Timeout, 8);
                    if (response.Status != null && response.Status.Count > 0)
                    {
                        outputStream.Write(response.Status, 9);
                    }
                    if (response.Context != null && response.Context.Count > 0)
                    {
                        outputStream.Write(response.Context, 10);
                    }
                    break;

                default:
                    break;
            }
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