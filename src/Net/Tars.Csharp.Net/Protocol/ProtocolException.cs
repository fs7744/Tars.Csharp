using System;

namespace Tars.Csharp.Net.Protocol
{
    public class ProtocolException : Exception
    {
        public ProtocolException() : base()
        {
        }

        public ProtocolException(string message) : base(message)
        {
        }

        public ProtocolException(string message, Exception cause) : base(message, cause)
        {
        }

        public ProtocolException(Exception cause) : this("", cause)
        {
        }
    }
}