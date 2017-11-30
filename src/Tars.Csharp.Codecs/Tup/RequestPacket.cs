using System.Collections.Generic;

namespace Tars.Csharp.Codecs.Tup
{
    public class RequestPacket
    {
        public short Version { get; set; }

        public byte PacketType { get; set; }

        public int MessageType { get; set; }

        public int RequestId { get; set; }

        public string ServantName { get; set; }

        public string FuncName { get; set; }

        public byte[] Buffer { get; set; }

        public int Timeout { get; set; }

        public IDictionary<string, string> Context { get; set; }

        public IDictionary<string, string> Status { get; set; }

        public int Ret { get; set; }

        public string ResultDesc { get; set; }

        public RequestPacket CreateResponse()
        {
            return new RequestPacket()
            {
                Version = Version,
                MessageType = MessageType,
                RequestId = RequestId,
                ServantName = ServantName,
                FuncName = FuncName,
                Timeout = Timeout,
            };
        }
    }
}