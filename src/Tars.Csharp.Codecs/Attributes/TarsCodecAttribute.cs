using DotNetty.Buffers;
using System.Collections.Generic;
using System.Linq;
using Tars.Csharp.Codecs.Tup;
using Tars.Csharp.Codecs.Util;

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
            result.Context = inputStream.ReadMap<string, string>(new Dictionary<string, string>(), 9, false);
            result.Status = inputStream.ReadMap<string, string>(new Dictionary<string, string>(), 10, false);
            input.MarkReaderIndex();

            return result;
        }

        public override RequestPacket DecodeResponse(IByteBuffer input)
        {
            var response = new RequestPacket();
            var tis = new TarsInputStream(input);

            response.Version = tis.Read((short)0, 1, true);
            response.PacketType = tis.Read((byte)0, 2, true);
            switch (response.Version)
            {
                case Const.Version:
                    response.RequestId = tis.Read(0, 3, true);
                    response.MessageType = tis.Read(0, 4, true);
                    response.Ret = tis.Read(0, 5, true);
                    response.Buffer = tis.Read(new byte[0], 6, true);
                    response.Status = tis.ReadMap<string, string>(new Dictionary<string, string>(), 7, false);
                    response.ResultDesc = tis.ReadString(8, false);
                    break;

                case Const.Version2:
                case Const.Version3:
                    response.MessageType = tis.Read(0, 3, true);
                    response.RequestId = tis.Read(0, 4, true);
                    response.ServantName = tis.ReadString(5, true);
                    response.FuncName = tis.ReadString(6, true);
                    response.Buffer = tis.Read(new byte[0], 7, true);
                    response.Timeout = tis.Read(0, 8, true);
                    response.Status = tis.ReadMap<string, string>(new Dictionary<string, string>(), 9, false);
                    response.Context = tis.ReadMap<string, string>(new Dictionary<string, string>(), 10, false);
                    break;

                default:
                    break;
            }
            return response;
        }

        public override object[] DecodeMethodParameters(RequestPacket request, RpcMethodMetadata metdata)
        {
            var buf = Unpooled.WrappedBuffer(request.Buffer);
            object[] ps = null;
            var stream = new TarsInputStream(buf);
            UniAttribute unaIn;
            switch (request.Version)
            {
                case Const.Version:
                    ps = metdata.Parameters.Select(i =>
                    {
                        object value = stream.Read(BasicClassTypeUtil.CreateObject(i.ParameterType), i.Position + 1, false);
                        //if (TarsHelper.isHolder(parameterInfo.getAnnotations()))
                        //{
                        //    value = new Holder<Object>(value);
                        //}
                        return value;
                    }).ToArray();
                    break;

                case Const.Version2:
                    unaIn = new UniAttribute();
                    //unaIn.setEncodeName(request.getCharsetName());
                    unaIn.DecodeTup2(stream);
                    ps = CreateParameters(unaIn, stream, metdata);
                    break;

                case Const.Version3:
                    unaIn = new UniAttribute();
                    //unaIn.setEncodeName(request.getCharsetName());
                    unaIn.DecodeTup3(stream);
                    ps = CreateParameters(unaIn, stream, metdata);
                    break;

                default:
                    //request.setRet(TarsHelper.SERVERDECODEERR);
                    break;
            }
            return ps;
        }

        private object[] CreateParameters(UniAttribute unaIn, TarsInputStream inputStream, RpcMethodMetadata metdata)
        {
            return metdata.Parameters.Select(i =>
            {
                object value = null;
                //if (TarsHelper.isHolder(parameterInfo.getAnnotations()))
                //{
                //    String holderName = TarsHelper.getHolderName(parameterInfo.getAnnotations());
                //    if (!StringUtils.isEmpty(holderName))
                //    {
                //        value = new Holder<Object>(unaIn.getByClass(holderName, parameterInfo.getStamp()));
                //    }
                //    else
                //    {
                //        value = new Holder<Object>();
                //    }
                //}
                //else
                //{
                value = unaIn.GetByClass(i.Name, BasicClassTypeUtil.CreateObject(i.ParameterType), inputStream);
                //}
                return value;
            }).ToArray();
        }

        public override object DecodeReturnValue(byte[] input, RpcMethodMetadata metdata)
        {
            var buf = Unpooled.WrappedBuffer(input);
            var stream = new TarsInputStream(buf);
            return stream.Read(BasicClassTypeUtil.CreateObject(metdata.ReturnInfo.ParameterType), metdata.ReturnInfo.Position + 1, true);
            //public void decodeResponseBody(ServantResponse resp) throws ProtocolException {
            //    TarsServantResponse response = (TarsServantResponse)resp;

            //    TarsServantRequest request = response.getRequest();
            //    if (request.isAsync())
            //    {
            //        return;
            //    }
            //    TarsInputStream is = response.getInputStream();

            //    byte[] data = is.read(new byte[] { }, 6, true);
            //    TarsInputStream jis = new TarsInputStream(data);
            //    jis.setServerEncoding(response.getCharsetName());

            //    TarsMethodInfo methodInfo = request.getMethodInfo();
            //    TarsMethodParameterInfo returnInfo = methodInfo.getReturnInfo();

            //    Object[] results;
            //    try
            //    {
            //        results = decodeResponseBody(data, response.getCharsetName(), methodInfo);
            //    }
            //    catch (Exception e)
            //    {
            //        throw new ProtocolException(e);
            //    }

            //    int i = 0;
            //    if (returnInfo != null && Void.TYPE != returnInfo.getType())
            //    {
            //        response.setResult(results[i++]);
            //    }

            //    List<TarsMethodParameterInfo> list = methodInfo.getParametersList();
            //    for (TarsMethodParameterInfo info : list)
            //    {
            //        if (!TarsHelper.isHolder(info.getAnnotations()))
            //        {
            //            continue;
            //        }
            //        try
            //        {
            //            TarsHelper.setHolderValue(request.getMethodParameters()[info.getOrder() - 1], results[i++]);
            //        }
            //        catch (Exception e)
            //        {
            //            throw new ProtocolException(e);
            //        }
            //    }
            //    response.setStatus((HashMap<String, String>) is.read(TarsHelper.STAMP_MAP, 7, false));
            //}

            //protected Object[] decodeResponseBody(byte[] data, String charset, TarsMethodInfo methodInfo) throws Exception {
            //    TarsMethodParameterInfo returnInfo = methodInfo.getReturnInfo();
            //    List<Object> values = new ArrayList<Object>();

            //    TarsInputStream jis = new TarsInputStream(data);
            //    jis.setServerEncoding(charset);

            //    if (returnInfo != null && Void.TYPE != returnInfo.getType())
            //    {
            //        values.add(jis.read(returnInfo.getStamp(), returnInfo.getOrder(), true));
            //    }

            //    List<TarsMethodParameterInfo> list = methodInfo.getParametersList();
            //    for (TarsMethodParameterInfo info : list)
            //    {
            //        if (!TarsHelper.isHolder(info.getAnnotations()))
            //        {
            //            continue;
            //        }
            //        try
            //        {
            //            values.add(jis.read(info.getStamp(), info.getOrder(), true));
            //        }
            //        catch (Exception e)
            //        {
            //            throw new ProtocolException(e);
            //        }
            //    }
            //    return values.toArray();
            //}
        }

        public override IByteBuffer EncodeRequest(RequestPacket request)
        {
            var buf = Unpooled.Buffer(128);
            var outputStream = new TarsOutputStream(buf);
            outputStream.Write(request.Version, 1);
            outputStream.Write(request.PacketType, 2);
            //outputStream.Write(request.RequestId, 3);
            //outputStream.Write(request.MessageType, 4);
            //outputStream.Write(request.Ret, 5);
            //outputStream.Write(request.Buffer, 6);
            //outputStream.Write(request.Status, 7);
            //outputStream.Write(request.ResultDesc == null ? "" : request.ResultDesc, 8);
            outputStream.Write(request.MessageType, 3);
            outputStream.Write(request.RequestId, 4);
            outputStream.Write(request.ServantName, 5);
            outputStream.Write(request.FuncName, 6);
            outputStream.Write(request.Buffer, 7);
            outputStream.Write(request.Timeout, 8);
            outputStream.Write(request.Context, 9);
            outputStream.Write(request.Status, 10);
            return buf.Slice();
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
            var buf = Unpooled.Buffer(128);
            var output = new TarsOutputStream(buf);
            foreach (var item in metdata.Parameters)
            {
                output.Write(parameters[item.Position], item.Position + 1);
            }
            return output.ToByteArray();
            //os.setServerEncoding(charsetName);

            //TarsMethodInfo methodInfo = request.getMethodInfo();
            //List<TarsMethodParameterInfo> parameterInfoList = methodInfo.getParametersList();

            //Object value = null;
            //Object[] parameter = request.getMethodParameters();
            //for (TarsMethodParameterInfo parameterInfo : parameterInfoList)
            //{
            //    if (TarsHelper.isContext(parameterInfo.getAnnotations()) || TarsHelper.isCallback(parameterInfo.getAnnotations()))
            //    {
            //        continue;
            //    }

            //    value = parameter[request.isAsync() ? parameterInfo.getOrder() : parameterInfo.getOrder() - 1];
            //    if (TarsHelper.isHolder(parameterInfo.getAnnotations()) && value != null)
            //    {
            //        try
            //        {
            //            value = TarsHelper.getHolderValue(value);
            //        }
            //        catch (Exception e)
            //        {
            //            throw new ProtocolException(e);
            //        }
            //        if (value != null)
            //        {
            //            os.write(value, parameterInfo.getOrder());
            //        }
            //    }
            //    else if (value != null)
            //    {
            //        os.write(value, parameterInfo.getOrder());
            //    }
            //}
            //return os.toByteArray();
        }

        public override byte[] EncodeReturnValue(object returnValue, RpcMethodMetadata metdata)
        {
            var buf = Unpooled.Buffer(128);
            var output = new TarsOutputStream(buf);
            output.Write(returnValue, 0);
            return output.ToByteArray();
            //if (TarsHelper.isPing(request.getFunctionName()))
            //{
            //    return new byte[] { };
            //}

            //if (ret == TarsHelper.SERVERSUCCESS && methodInfoMap != null)
            //{
            //    TarsMethodInfo methodInfo = methodInfoMap.get(request.getFunctionName());
            //    TarsMethodParameterInfo returnInfo = methodInfo.getReturnInfo();
            //    if (returnInfo != null && returnInfo.getType() != Void.TYPE && response.getResult() != null)
            //    {
            //        try
            //        {
            //            ajos.write(response.getResult(), methodInfo.getReturnInfo().getOrder());
            //        }
            //        catch (Exception e)
            //        {
            //            System.err.println("server encodec response result:" + response.getResult() + " with ex:" + e);
            //        }
            //    }

            //    Object value = null;
            //    List<TarsMethodParameterInfo> parametersList = methodInfo.getParametersList();
            //    for (TarsMethodParameterInfo parameterInfo : parametersList)
            //    {
            //        if (TarsHelper.isHolder(parameterInfo.getAnnotations()))
            //        {
            //            value = request.getMethodParameters()[parameterInfo.getOrder() - 1];
            //            if (value != null)
            //            {
            //                try
            //                {
            //                    ajos.write(TarsHelper.getHolderValue(value), parameterInfo.getOrder());
            //                }
            //                catch (Exception e)
            //                {
            //                    System.err.println("server encodec response holder:" + value + " with ex:" + e);
            //                }
            //            }
            //        }
            //    }
            //}
            //return ajos.toByteArray();

            //TarsServantRequest request = response.getRequest();
            //UniAttribute unaOut = new UniAttribute();
            //unaOut.setEncodeName(charsetName);
            //if (response.getVersion() == TarsHelper.VERSION3)
            //{
            //    unaOut.useVersion3();
            //}

            //int ret = response.getRet();
            //Map<String, TarsMethodInfo> methodInfoMap = AnalystManager.getInstance().getMethodMapByName(request.getServantName());
            //if (ret == TarsHelper.SERVERSUCCESS && methodInfoMap != null)
            //{
            //    TarsMethodInfo methodInfo = methodInfoMap.get(request.getFunctionName());
            //    TarsMethodParameterInfo returnInfo = methodInfo.getReturnInfo();
            //    if (returnInfo != null && returnInfo.getType() != Void.TYPE && response.getResult() != null)
            //    {
            //        unaOut.put(TarsHelper.STAMP_STRING, response.getResult());
            //    }

            //    Object value = null;
            //    List<TarsMethodParameterInfo> parametersList = methodInfo.getParametersList();
            //    for (TarsMethodParameterInfo parameterInfo : parametersList)
            //    {
            //        if (TarsHelper.isHolder(parameterInfo.getAnnotations()))
            //        {
            //            value = request.getMethodParameters()[parameterInfo.getOrder() - 1];
            //            if (value != null)
            //            {
            //                try
            //                {
            //                    String holderName = TarsHelper.getHolderName(parameterInfo.getAnnotations());
            //                    if (!StringUtils.isEmpty(holderName))
            //                    {
            //                        unaOut.put(holderName, TarsHelper.getHolderValue(value));
            //                    }
            //                }
            //                catch (Exception e)
            //                {
            //                    System.err.println("server encodec response holder:" + value + " with ex:" + e);
            //                }
            //            }
            //        }
            //    }
            //}
            //return unaOut.encode();
        }
    }
}