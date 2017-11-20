using System;

namespace Tars.Csharp.Core.Protocol.Tup
{
    public class ObjectCreateException : Exception
    {
        public ObjectCreateException(Exception ex)
            : base("ObjectCreateException", ex)
        {
        }
    }
}