namespace Tars.Csharp.Net.Protocol
{
    public interface IProtocolFactory
    {
        ProtocolEncoder GetEncoder();

        ProtocolDecoder GetDecoder();
    }
}