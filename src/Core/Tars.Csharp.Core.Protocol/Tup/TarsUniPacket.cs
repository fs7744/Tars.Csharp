using System;
using System.Collections.Generic;
using Tars.Csharp.Core.Protocol.Util;

namespace Tars.Csharp.Core.Protocol.Tup
{
    /// <summary>
    /// 通过TUP调用TARS需要使用的包类型
    /// </summary>
    public class TarsUniPacket : UniPacket
    {
        public TarsUniPacket()
        {
            package.Version = (short)2;
            package.PacketType = Const.PacketTypeTup;
            package.MessageType = (int)0;
            package.Timeout = (int)0;
            package.Buffer = new byte[] { };
            package.Context = new Dictionary<string, string>();
            package.Status = new Dictionary<string, string>();
        }

        public byte PacketType { get => package.PacketType; set => package.PacketType = value; }

        public int MessageType { get => package.MessageType; set => package.MessageType = value; }

        public int Timeout { get => package.Timeout; set => package.Timeout = value; }

        public byte[] Buffer { get => package.Buffer; set => package.Buffer = value; }

        public Dictionary<string, string> Context { get => package.Context; set => package.Context = value; }

        public Dictionary<string, string> Status { get => package.Status; set => package.Status = value; }

        public int GetTarsResultCode()
        {
            int result = 0;
            try
            {
                string rcode = package.Status[(Const.StatusResultCode)];
                result = (rcode != null ? int.Parse(rcode) : 0);
            }
            catch (Exception e)
            {
                QTrace.Trace(e.Message);
                return 0;
            }
            return result;
        }

        public string GetTarsResultDesc()
        {
            string rdesc = package.Status[(Const.StatusResultDesc)];
            string result = rdesc != null ? rdesc : "";
            return result;
        }
    }
}