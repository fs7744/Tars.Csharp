using System;
using System.Collections.Generic;
using System.Reflection;
using Tars.Csharp.Codecs.Attributes;
using AspectCore.Extensions.Reflection;

namespace Tars.Csharp.Codecs
{
    public class RpcMetadata
    {
        public Type InterfaceType { get; set; }

        public string Servant { get; set; }

        public CodecAttribute Codec { get; set; }

        public Dictionary<string, RpcMethodMetadata> Methods { get; set; }

        public Type ServantType { get; set; }

        public object ServantInstance { get; set; }

        public Type CodecType { get; set; }
    }

    public class RpcMethodMetadata
    {
        public string Name { get; set; }
        public ParameterInfo ReturnInfo { get; set; }
        public ParameterInfo[] Parameters { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public MethodReflector Reflector { get; set; }
    }
}