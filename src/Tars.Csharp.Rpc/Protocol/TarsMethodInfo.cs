using DotNetty.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tars.Csharp.Codecs;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Rpc.Protocol
{
    public class TarsMethodInfo
    {
        public string ServiceName { get; set; }

        public MethodBase Method { get; set; }

        public string Comment { get; set; }

        public List<TarsMethodParameterInfo> Parameters { get; set; }

        public TarsMethodParameterInfo ReturnInfo { get; set; }

        public object[] ReadFrom(RequestPacket request)
        {
            //if (methodInfoMap == null || methodInfoMap.isEmpty())
            //{
            //    request.setRet(TarsHelper.SERVERNOSERVANTERR);
            //    throw new ProtocolException("no found methodInfo, the context[ROOT], serviceName[" + servantName + "], methodName[" + methodName + "]");
            //}
            //TarsMethodInfo methodInfo = methodInfoMap.get(methodName);
            //if (methodInfo == null)
            //{
            //    request.setRet(TarsHelper.SERVERNOFUNCERR);
            //    throw new ProtocolException("no found methodInfo, the context[ROOT], serviceName[" + servantName + "], methodName[" + methodName + "]");
            //}

            var buf = Unpooled.WrappedBuffer(request.Buffer);
            object[] ps = null;
            try
            {
                var stream = new TarsInputStream(buf);
                UniAttribute unaIn;
                switch (request.Version)
                {
                    case 0x01:
                        ps = DecodeRequestBody(stream);
                        break;

                    case 0x02:
                        unaIn = new UniAttribute();
                        //unaIn.setEncodeName(request.getCharsetName());
                        unaIn.DecodeTup2(stream);
                        ps = CreateParameters(unaIn, stream);
                        break;

                    case 0x03:
                        unaIn = new UniAttribute();
                        //unaIn.setEncodeName(request.getCharsetName());
                        unaIn.DecodeTup3(stream);
                        ps = CreateParameters(unaIn, stream);
                        break;

                    default:
                        //request.setRet(TarsHelper.SERVERDECODEERR);
                        break;
                }
            }
            finally
            {
                buf.Release();
            }

            return ps;
        }

        private object[] CreateParameters(UniAttribute unaIn, TarsInputStream inputStream)
        {
            return Parameters.Select(i =>
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
                value = unaIn.GetByClass(i.Name, i.Stamp, inputStream);
                //}
                return value;
            }).ToArray();
        }

        private object[] DecodeRequestBody(TarsInputStream stream)
        {
            return Parameters.Select(i =>
            {
                object value = stream.Read(i.Stamp, i.Order, false);
                //if (TarsHelper.isHolder(parameterInfo.getAnnotations()))
                //{
                //    value = new Holder<Object>(value);
                //}
                return value;
            }).ToArray();
        }
    }
}