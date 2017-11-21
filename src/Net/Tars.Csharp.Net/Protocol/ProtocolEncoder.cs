using Tars.Csharp.Net.Core;

namespace Tars.Csharp.Net.Protocol
{
    public interface ProtocolEncoder
    {
        IoBuffer EncodeResponse(Response response, Session session);

        IoBuffer EncodeRequest(Request request, Session session);
    }
}