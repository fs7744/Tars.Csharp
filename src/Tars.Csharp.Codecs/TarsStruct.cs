namespace Tars.Csharp.Codecs
{
    public abstract class TarsStruct
    {
        public static int TarsMaxStringLength = 100 * 1024 * 1024;

        public abstract void WriteTo(TarsOutputStream outputStream);

        public abstract void ReadFrom(TarsInputStream inputStream);
    }
}