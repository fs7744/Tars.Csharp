using System;

namespace Tars.Csharp.Rpc.Protocol
{
    public class TarsMethodParameterInfo
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public object Stamp { get; set; }
        public Type Type { get; set; }
        public Attribute Attributes { get; set; }
    }
}