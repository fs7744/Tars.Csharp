namespace Tars.Csharp.Net.Core
{
    public abstract class Processor
    {
        public abstract Response Process(Request request, Session session);
    }
}