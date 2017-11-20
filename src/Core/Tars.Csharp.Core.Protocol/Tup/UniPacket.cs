using System;
using System.Collections.Generic;
using System.IO;
using Tars.Csharp.Core.Protocol.Util;

namespace Tars.Csharp.Core.Protocol.Tup
{
    public class UniPacket : UniAttribute
    {
        public static readonly int UniPacketHeadSize = 4;

        protected RequestPacket package = new RequestPacket() { Version = Const.PacketTypeTup };

        public string ServantName { get => package.ServantName; set => package.ServantName = value; }

        public string FuncName { get => package.FuncName; set => package.FuncName = value; }

        public int RequestId { get => package.RequestId; set => package.RequestId = value; }

        public new short Version { get => package.Version; set => package.Version = value; }

        public new bool IsPacketTypeTup3 => package.Version == Const.PacketTypeTup3;

        public int OldRespIRet { get; set; }

        public override void ReadFrom(TarsInputStream inputStream)
        {
            package.ReadFrom(inputStream);
        }

        public override void WriteTo(TarsOutputStream outputStream)
        {
            package.WriteTo(outputStream);
        }

        /// <summary>
        /// 为兼容最早发布的客户端版本解码使用 iret字段始终为0
        /// </summary>
        /// <returns></returns>
        public byte[] CreateOldRespEncode()
        {
            TarsOutputStream os = new TarsOutputStream(0);
            os.SetServerEncoding(EncodeName);
            os.Write(data, 0);
            byte[] oldSBuffer = TarsUtil.GetTarsBufArray(os.GetMemoryStream());
            os = new TarsOutputStream(0);
            os.SetServerEncoding(EncodeName);
            os.Write(package.Version, 1);
            os.Write(package.PacketType, 2);
            os.Write(package.RequestId, 3);
            os.Write(package.MessageType, 4);
            os.Write(OldRespIRet, 5);
            os.Write(oldSBuffer, 6);
            os.Write(package.Status, 7);
            return TarsUtil.GetTarsBufArray(os.GetMemoryStream());
        }

        /// <summary>
        /// 将put的对象进行编码
        /// </summary>
        /// <returns></returns>
        public new byte[] Encode()
        {
            if (package.ServantName.Equals(""))
            {
                throw new ArgumentException("servantName can not is null");
            }
            if (package.FuncName.Equals(""))
            {
                throw new ArgumentException("funcName can not is null");
            }

            TarsOutputStream os = new TarsOutputStream(0);
            os.SetServerEncoding(EncodeName);
            if (IsPacketTypeTup3)
            {
                os.Write(newData, 0);
            }
            else
            {
                os.Write(data, 0);
            }

            package.Buffer = TarsUtil.GetTarsBufArray(os.GetMemoryStream());

            os = new TarsOutputStream(0);
            os.SetServerEncoding(EncodeName);
            this.WriteTo(os);
            byte[] bodys = TarsUtil.GetTarsBufArray(os.GetMemoryStream());
            int size = bodys.Length;

            MemoryStream ms = new MemoryStream(size + UniPacketHeadSize);
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                // 整个数据包长度
                bw.Write(ByteConverter.ReverseEndian(size + UniPacketHeadSize));
                bw.Write(bodys);
            }

            return ms.ToArray();
        }

        public new void Decode(byte[] buffer, int Index = 0)
        {
            if (buffer.Length < UniPacketHeadSize)
            {
                throw new ArgumentException("Decode namespace must include size head");
            }

            TarsInputStream tis = new TarsInputStream(buffer, UniPacketHeadSize + Index);
            tis.SetServerEncoding(EncodeName);
            //解码出RequestPacket包
            this.ReadFrom(tis);

            tis = new TarsInputStream(package.Buffer);
            tis.SetServerEncoding(EncodeName);
            if (IsPacketTypeTup3)
            {
                newData = (Dictionary<string, byte[]>)tis.ReadMap<Dictionary<string, byte[]>>(0, false);
            }
            else
            {
                data = (Dictionary<string, Dictionary<string, byte[]>>)tis.ReadMap<Dictionary<string, Dictionary<string, byte[]>>>(0, false);
            }
        }
    }
}