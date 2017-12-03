using Tars.Csharp.Codecs.Attributes;
using Tars.Csharp.Rpc;

namespace Hello.Common
{
    [Rpc(Servant = "TestApp.HelloServer.HelloObj")]
    [TarsCodec]
    public interface IHelloRpc
    {
        string Hello(int no, string name);

        void HelloHolder(int no, out string name);
    }
}