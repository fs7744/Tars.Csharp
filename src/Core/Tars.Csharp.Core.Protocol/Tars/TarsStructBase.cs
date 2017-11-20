using System.Text;

namespace Tars.Csharp.Core.Protocol
{
    public abstract class TarsStructBase
    {
        public static int TarsMaxStringLength = 100 * 1024 * 1024;

        public abstract void WriteTo(TarsOutputStream outputStream);

        public abstract void ReadFrom(TarsInputStream inputStream);

        public void Display(StringBuilder sb, int level)
        {
        }

        public void DisplaySimple(StringBuilder sb, int level)
        {
        }

        public TarsStructBase NewInit()
        {
            return null;
        }

        public void Recyle()
        {
        }

        public bool ContainField(string name)
        {
            return false;
        }

        public object GetFieldByName(string name)
        {
            return null;
        }

        public void SetFieldByName(string name, object value)
        {
        }

        public byte[] ToByteArray()
        {
            var os = new TarsOutputStream();
            WriteTo(os);
            return os.ToByteArray();
        }

        public byte[] ToByteArray(string encoding)
        {
            var os = new TarsOutputStream();
            os.SetServerEncoding(encoding);
            WriteTo(os);
            return os.ToByteArray();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            Display(sb, 0);
            return sb.ToString();
        }

        public static string ToDisplaySimpleString(TarsStructBase structData)
        {
            if (structData == null)
            {
                return null;
            }
            var sb = new StringBuilder();
            structData.DisplaySimple(sb, 0);
            return sb.ToString();
        }
    }
}