using Tars.Csharp.Net.Core;

namespace Tars.Csharp.Net.Protocol
{
    public interface ProtocolDecoder
    {
        Request DecodeRequest(IoBuffer buff, Session session);

        Response DecodeResponse(IoBuffer buff, Session session);
    }
}