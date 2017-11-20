namespace Tars.Csharp.Core.Protocol.Tup
{
    internal class Const
    {
        public static string StatusGridKey = "STATUS_GRID_KEY";
        public static string StatusDyedKey = "STATUS_DYED_KEY";
        public static string StatusGridCode = "STATUS_GRID_CODE";
        public static string StatusSampleKey = "STATUS_SAMPLE_KEY";
        public static string StatusResultCode = "STATUS_RESULT_CODE";
        public static string StatusResultDesc = "STATUS_RESULT_DESC";

        public static int InvalidHashCode = -1;
        public static int InvalidGridCode = -1;

        public static byte PacketTypeTarsnormal = 0;
        public static byte PacketTypeTarsoneway = 1;
        public static byte PacketTypeTup = 2;
        public static byte PacketTypeTup3 = 3;
    }
}