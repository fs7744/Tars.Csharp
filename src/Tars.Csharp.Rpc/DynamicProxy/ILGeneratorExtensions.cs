using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Linq;

namespace Tars.Csharp.Rpc.DynamicProxy
{
    public static class ILGeneratorExtensions
    {
        public static void EmitThis(this ILGenerator ilGenerator)
        {
            if (ilGenerator == null)
            {
                throw new ArgumentNullException(nameof(ilGenerator));
            }

            ilGenerator.EmitLoadArg(0);
        }

        public static void EmitLoadArg(this ILGenerator ilGenerator, int index)
        {
            if (ilGenerator == null)
            {
                throw new ArgumentNullException(nameof(ilGenerator));
            }

            switch (index)
            {
                case 0:
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index <= byte.MaxValue) ilGenerator.Emit(OpCodes.Ldarg_S, (byte)index);
                    else ilGenerator.Emit(OpCodes.Ldarg, index);
                    break;
            }
        }

        public static void EmitDefault(this ILGenerator ilGenerator, Type type)
        {
            if (ilGenerator == null)
            {
                throw new ArgumentNullException(nameof(ilGenerator));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                case TypeCode.DateTime:
                    if (type.GetTypeInfo().IsValueType)
                    {
                        // Type.GetTypeCode on an enum returns the underlying
                        // integer TypeCode, so we won't get here.
                        // This is the IL for default(T) if T is a generic type
                        // parameter, so it should work for any type. It's also
                        // the standard pattern for structs.
                        LocalBuilder lb = ilGenerator.DeclareLocal(type);
                        ilGenerator.Emit(OpCodes.Ldloca, lb);
                        ilGenerator.Emit(OpCodes.Initobj, type);
                        ilGenerator.Emit(OpCodes.Ldloc, lb);
                    }
                    else
                    {
                        ilGenerator.Emit(OpCodes.Ldnull);
                    }
                    break;

                case TypeCode.Empty:
                case TypeCode.String:
                    ilGenerator.Emit(OpCodes.Ldnull);
                    break;

                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    break;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    ilGenerator.Emit(OpCodes.Conv_I8);
                    break;

                case TypeCode.Single:
                    ilGenerator.Emit(OpCodes.Ldc_R4, default(Single));
                    break;

                case TypeCode.Double:
                    ilGenerator.Emit(OpCodes.Ldc_R8, default(Double));
                    break;

                case TypeCode.Decimal:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    ilGenerator.Emit(OpCodes.Newobj, typeof(Decimal).GetTypeInfo().GetConstructor(new Type[] { typeof(int) }));
                    break;

                default:
                    throw new InvalidOperationException("Code supposed to be unreachable.");
            }
        }



        private static readonly ConcurrentDictionary<MethodInfo, PropertyInfo> dictionary = new ConcurrentDictionary<MethodInfo, PropertyInfo>();

        public static bool IsPropertyBinding(this MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            return method.GetBindingProperty() != null;
        }

        public static PropertyInfo GetBindingProperty(this MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return dictionary.GetOrAdd(method, m =>
            {
                foreach (var property in m.DeclaringType.GetTypeInfo().GetProperties())
                {
                    if (property.CanRead && property.GetMethod == m)
                    {
                        return property;
                    }

                    if (property.CanWrite && property.SetMethod == m)
                    {
                        return property;
                    }
                }
                return null;
            });
        }

        public static Type[] GetParameterTypes(this MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            return method.GetParameters().Select(parame => parame.ParameterType).ToArray();
        }
    }
}
