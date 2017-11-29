using System;
using System.Collections.Generic;
using System.Reflection;
using Tars.Csharp.Attributes;

namespace Tars.Csharp.Codecs
{
    public class RpcMetadata
    {
        public Type InterfaceType { get; set; }

        public CodecAttribute Codec { get; set; }

        public Dictionary<string, RpcMethodMetadata> Methods { get; set; }
    }

    public class RpcMethodMetadata
    {
        public string Name { get; set; }
        public ParameterInfo ReturnInfo { get; set; }
        public ParameterInfo[] Parameters { get; set; }
    }
}
