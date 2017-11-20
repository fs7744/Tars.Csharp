using System;

namespace Tars.Csharp.Core.Protocol.Tars.Exceptions
{
    public class TarsDecodeException : Exception
    {
        public TarsDecodeException(string message) : base(message)
        {
        }
    }
}