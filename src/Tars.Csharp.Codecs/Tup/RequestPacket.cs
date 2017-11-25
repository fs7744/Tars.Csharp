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

        public Dictionary<string, string> Context { get; set; }

        public Dictionary<string, string> Status { get; set; }

        public override void ReadFrom(TarsInputStream inputStream)
        {
            Version = inputStream.Read(Version, 1, true);
            PacketType = inputStream.Read(PacketType, 2, true);
            MessageType = inputStream.Read(MessageType, 3, true);
            RequestId = inputStream.Read(RequestId, 4, true);
            ServantName = inputStream.ReadString(5, true);
            FuncName = inputStream.ReadString(6, true);

            var cache_sBuffer = new byte[] { 0 };
            Buffer = (byte[])inputStream.Read<byte[]>(cache_sBuffer, 7, true);
            Timeout = inputStream.Read(Timeout, 8, true);

            Dictionary<string, string> cache_context = null;
            Context = (Dictionary<string, string>)inputStream.Read(cache_context, 9, true);
            Status = (Dictionary<string, string>)inputStream.Read(cache_context, 10, true);
        }

        //public override void WriteTo(TarsOutputStream outputStream)
        //{
        //    outputStream.Write(Version, 1);
        //    outputStream.Write(PacketType, 2);
        //    outputStream.Write(MessageType, 3);
        //    outputStream.Write(RequestId, 4);
        //    outputStream.Write(ServantName, 5);
        //    outputStream.Write(FuncName, 6);
        //    outputStream.Write(Buffer, 7);
        //    outputStream.Write(Timeout, 8);
        //    outputStream.Write(Context, 9);
        //    outputStream.Write(Status, 10);
        //}
    }
}