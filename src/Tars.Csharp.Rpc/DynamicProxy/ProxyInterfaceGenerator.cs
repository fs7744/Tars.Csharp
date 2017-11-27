using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Linq;

namespace Tars.Csharp.Rpc.DynamicProxy
{
    public class ProxyInterfaceGenerator
    {
        private const string ProxyNameSpace = "Tars.Csharp.Rpc.DynamicProxy";
        private const string ProxyAssemblyName = "Tars.Csharp.Rpc.DynamicProxy.Core";
        private const string ContextName = "Context";
        private readonly ModuleBuilder moduleBuilder;
        internal static readonly ConstructorInfo ObjectCtor = typeof(object).GetTypeInfo().DeclaredConstructors.Single();
        internal const MethodAttributes InterfaceMethodAttributes = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
        static readonly ConstructorInfo DynamicProxyContextCtor = typeof(DynamicProxyContext).GetTypeInfo().DeclaredConstructors.First();

        private readonly object _lock = new object();
        private readonly Dictionary<string, Type> definedTypes = new Dictionary<string, Type>();

        public ProxyInterfaceGenerator()
        {
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(ProxyAssemblyName), AssemblyBuilderAccess.RunAndCollect);
            moduleBuilder = asmBuilder.DefineDynamicModule("Impl");
        }

        public Type CreateInterfaceProxyType(Type interfaceType)
        {
            var typeInfo = interfaceType.GetTypeInfo();
            if (!typeInfo.IsInterface || !typeInfo.IsVisible)
            {
                throw new ArgumentException($"Type '{interfaceType}' should be Visible interface.", nameof(interfaceType));
            }

            lock (_lock)
            {
                var name = GetProxyImplTypeFullName(interfaceType);
                if (!definedTypes.TryGetValue(name, out Type type))
                {
                    type = CreateProxyInternal(name, interfaceType);
                    definedTypes[name] = type;
                }
                return type;
            }
        }

        private Type CreateProxyInternal(string name, Type interfaceType)
        {
            throw new NotImplementedException();
        }

        private string GetProxyImplTypeFullName(Type interfaceType)
        {
            return $"{ProxyNameSpace}.{interfaceType.Name}Impl";
        }

        private Type CreateInterfaceProxyInternal(string name, Type interfaceType)
        {
            var interfaces = new Type[] { interfaceType };
            //define proxy type for interface service
            var typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed, typeof(object), interfaces);

            typeBuilder.DefineField(ContextName, typeof(DynamicProxyContext), FieldAttributes.Public);

            //define constructor
            DefineInterfaceProxyConstructor(typeBuilder);

            //define methods
            DefineInterfaceProxyMethods(interfaceType, typeBuilder);

            return typeBuilder.CreateTypeInfo().AsType();
        }

        private void DefineInterfaceProxyConstructor(TypeBuilder typeBuilder)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, ObjectCtor.CallingConvention, Type.EmptyTypes);
            var ilGen = constructorBuilder.GetILGenerator();
            ilGen.EmitThis();
            ilGen.Emit(OpCodes.Call, ObjectCtor);
            ilGen.Emit(OpCodes.Ret);
        }

        private void DefineInterfaceProxyMethods(Type interfaceType,TypeBuilder typeBuilder)
        {
            foreach (var method in interfaceType.GetTypeInfo().DeclaredMethods.Where(x => !x.IsPropertyBinding()))
            {
                //var methodBuilder = DefineMethod(method, method.Name, InterfaceMethodAttributes, interfaceType, typeBuilder);
                //typeBuilder.DefineMethodOverride(methodBuilder, method);
            }
        }

        //private MethodBuilder DefineMethod(MethodInfo method, string name, MethodAttributes attributes, Type implType, TypeBuilder typeBuilder)
        //{
        //    var methodBuilder = typeBuilder.DefineMethod(name, attributes, method.CallingConvention, method.ReturnType, method.GetParameterTypes());

        //    DefineParameters(method, methodBuilder);

        //    EmitProxyMethodBody();

        //    return methodBuilder;

        //    void EmitProxyMethodBody()
        //    {
        //        var ilGen = methodBuilder.GetILGenerator();
        //        var activatorContext = ilGen.DeclareLocal(typeof(DynamicProxyContext));
        //        var returnValue = default(LocalBuilder);

        //        EmitInitializeMetaData(ilGen);

        //        ilGen.Emit(OpCodes.Newobj, DynamicProxyContextCtor);
        //        ilGen.Emit(OpCodes.Stloc, activatorContext);

        //        ilGen.EmitThis();

        //        EmitReturnVaule(ilGen);

        //        if (method.ReturnType != typeof(void))
        //        {
        //            returnValue = ilGen.DeclareLocal(method.ReturnType);
        //            ilGen.Emit(OpCodes.Stloc, returnValue);
        //        }

        //        var parameterTypes = method.GetParameterTypes();

        //        if (parameterTypes.Any(x => x.IsByRef))
        //        {
        //            var parameters = ilGen.DeclareLocal(typeof(object[]));
        //            ilGen.Emit(OpCodes.Ldloca, activatorContext);
        //            ilGen.Emit(OpCodes.Call, MethodUtils.GetParameters);
        //            ilGen.Emit(OpCodes.Stloc, parameters);
        //            for (var i = 0; i < parameterTypes.Length; i++)
        //            {
        //                if (parameterTypes[i].IsByRef)
        //                {
        //                    ilGen.EmitLoadArg(i + 1);
        //                    ilGen.Emit(OpCodes.Ldloc, parameters);
        //                    ilGen.EmitInt(i);
        //                    ilGen.Emit(OpCodes.Ldelem_Ref);
        //                    ilGen.EmitConvertFromObject(parameterTypes[i].GetElementType());
        //                    ilGen.EmitStRef(parameterTypes[i]);
        //                }
        //            }
        //        }

        //        if (returnValue != null)
        //        {
        //            ilGen.Emit(OpCodes.Ldloc, returnValue);
        //        }
        //        ilGen.Emit(OpCodes.Ret);
        //    }

        //    void EmitInitializeMetaData(ILGenerator ilGen)
        //    {
        //        var serviceMethod = method;

        //        ilGen.EmitThis();
        //        var parameterTypes = method.GetParameterTypes();
        //        if (parameterTypes.Length == 0)
        //        {
        //            ilGen.Emit(OpCodes.Ldnull);
        //            return;
        //        }
        //        ilGen.EmitInt(parameterTypes.Length);
        //        ilGen.Emit(OpCodes.Newarr, typeof(object));
        //        for (var i = 0; i < parameterTypes.Length; i++)
        //        {
        //            ilGen.Emit(OpCodes.Dup);
        //            ilGen.EmitInt(i);
        //            ilGen.EmitLoadArg(i + 1);
        //            if (parameterTypes[i].IsByRef)
        //            {
        //                ilGen.EmitLdRef(parameterTypes[i]);
        //                ilGen.EmitConvertToObject(parameterTypes[i].GetElementType());
        //            }
        //            else
        //            {
        //                ilGen.EmitConvertToObject(parameterTypes[i]);
        //            }
        //            ilGen.Emit(OpCodes.Stelem_Ref);
        //        }
        //    }

        //    void EmitReturnVaule(ILGenerator ilGen)
        //    {
        //        if (method.ReturnType == typeof(void))
        //        {
        //            ilGen.Emit(OpCodes.Callvirt, MethodUtils.AspectActivatorInvoke.MakeGenericMethod(typeof(object)));
        //            ilGen.Emit(OpCodes.Pop);
        //        }
        //        else if (method.ReturnType == typeof(Task))
        //        {
        //            ilGen.Emit(OpCodes.Callvirt, MethodUtils.AspectActivatorInvokeTask.MakeGenericMethod(typeof(object)));
        //        }
        //        else if (method.IsReturnTask())
        //        {
        //            var returnType = method.ReturnType.GetTypeInfo().GetGenericArguments().Single();
        //            ilGen.Emit(OpCodes.Callvirt, MethodUtils.AspectActivatorInvokeTask.MakeGenericMethod(returnType));
        //        }
        //        else if (method.IsReturnValueTask())
        //        {
        //            var returnType = method.ReturnType.GetTypeInfo().GetGenericArguments().Single();
        //            ilGen.Emit(OpCodes.Callvirt, MethodUtils.AspectActivatorInvokeValueTask.MakeGenericMethod(returnType));
        //        }
        //        else
        //        {
        //            ilGen.Emit(OpCodes.Callvirt, MethodUtils.AspectActivatorInvoke.MakeGenericMethod(method.ReturnType));
        //        }
        //    }
        //}

        private void DefineParameters(MethodInfo targetMethod, MethodBuilder methodBuilder)
        {
            var parameters = targetMethod.GetParameters();
            if (parameters.Length > 0)
            {
                var paramOffset = 1;   // 1
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var parameterBuilder = methodBuilder.DefineParameter(i + paramOffset, parameter.Attributes, parameter.Name);
                    if (parameter.HasDefaultValue)
                    {
                        if (!(parameter.ParameterType.GetTypeInfo().IsValueType && parameter.DefaultValue == null))
                            parameterBuilder.SetConstant(parameter.DefaultValue);
                    }
                }
            }

            var returnParamter = targetMethod.ReturnParameter;
            var returnParameterBuilder = methodBuilder.DefineParameter(0, returnParamter.Attributes, returnParamter.Name);
        }
    }
}
