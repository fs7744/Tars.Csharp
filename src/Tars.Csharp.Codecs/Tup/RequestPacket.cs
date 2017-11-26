using System.Collections.Generic;

namespace Tars.Csharp.Codecs.Tup
{
    public class RequestPacket : TarsStruct
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

        public override void ReadFrom(TarsInputStream inputStream)
        {
            Version = inputStream.Read(Version, 1, true);
            PacketType = inputStream.Read(PacketType, 2, true);
            MessageType = inputStream.Read(MessageType, 3, true);
            RequestId = inputStream.Read(RequestId, 4, true);
            ServantName = inputStream.ReadString(5, true);
            FuncName = inputStream.ReadString(6, true);

            Buffer = inputStream.ReadBytes(7, true);
            Timeout = inputStream.Read(Timeout, 8, true);

            Context = inputStream.ReadMap<string, string>(new Dictionary<string, string>(), 9, true);
            Status = inputStream.ReadMap<string, string>(new Dictionary<string, string>(), 10, true);
        }

        public override void WriteTo(TarsOutputStream outputStream)
        {
            outputStream.Write(Version, 1);
            outputStream.Write(PacketType, 2);
            outputStream.Write(MessageType, 3);
            outputStream.Write(RequestId, 4);
            outputStream.Write(ServantName, 5);
            outputStream.Write(FuncName, 6);
            outputStream.Write(Buffer, 7);
            outputStream.Write(Timeout, 8);
            outputStream.Write(Context, 9);
            outputStream.Write(Status, 10);
        }
    }
}