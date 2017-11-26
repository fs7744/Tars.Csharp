using System.Collections.Generic;

namespace Tars.Csharp.Codecs.Tup
{
    public class ResponsePacket : TarsStruct
    {
        public short Version { get; set; }

        public byte PacketType { get; set; }

        public int RequestId { get; set; }

        public int MessageType { get; set; }

        public int Ret { get; set; }

        public byte[] Buffer { get; set; }

        public IDictionary<string, string> Status { get; set; }

        public string ResultDesc { get; set; }

        public IDictionary<string, string> Context { get; set; }

        public override void ReadFrom(TarsInputStream inputStream)
        {
            Version = inputStream.Read(Version, 1, true);
            PacketType = inputStream.Read(PacketType, 2, true);
            RequestId = inputStream.Read(RequestId, 3, true);
            MessageType = inputStream.Read(MessageType, 4, true);
            Ret = inputStream.Read(Ret, 5, true);
            Buffer = inputStream.ReadBytes(6, true);
            Status = inputStream.ReadMap<string, string>(new Dictionary<string, string>(), 7, true);
            ResultDesc = inputStream.ReadString(8, false);
            Context = inputStream.ReadMap<string, string>(new Dictionary<string, string>(), 9, false);
        }

        public override void WriteTo(TarsOutputStream outputStream)
        {
            outputStream.Write(Version, 1);
            outputStream.Write(PacketType, 2);
            outputStream.Write(RequestId, 3);
            outputStream.Write(MessageType, 4);
            outputStream.Write(Ret, 5);
            outputStream.Write(Buffer, 6);
            outputStream.Write(Status, 7);
            outputStream.Write(ResultDesc, 8);
            outputStream.Write(Context, 9);
        }
    }
}