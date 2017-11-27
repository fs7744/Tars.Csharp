using System;

namespace Tars.Csharp.Rpc.Attributes
{
    public class RpcAttribute : Attribute
    {
        public Codec Codec { get; set; }
    }
}