/**
 * Tencent is pleased to support the open source community by making Tars available.
 *
 * Copyright (C) 2016THL A29 Limited, a Tencent company. All rights reserved.
 *
 * Licensed under the BSD 3-Clause License (the "License"); you may not use this file except 
 * in compliance with the License. You may obtain a copy of the License at
 *
 * https://opensource.org/licenses/BSD-3-Clause
 *
 * Unless required by applicable law or agreed to in writing, software distributed 
 * under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
 * CONDITIONS OF ANY KIND, either express or implied. See the License for the 
 * specific language governing permissions and limitations under the License.
 */

using System;

namespace Tars.Csharp.Core.Protocol.Util
{
    internal class BasicClassTypeUtil
    {
        public static object CreateObject<T>()
        {
            return CreateObject(typeof(T));
        }

        public static object CreateObject(Type type)
        {
            try
            {
                // String类型没有缺少构造函数，
                if (type.ToString() == "System.String")
                {
                    return "";
                }
                else if (type == typeof(byte[]))
                {
                    return new byte[0];
                }
                else if (type == typeof(short[]))
                {
                    return new short[0];
                }
                else if (type == typeof(ushort[]))
                {
                    return new ushort[0];
                }
                else if (type == typeof(int[]))
                {
                    return new int[0];
                }
                else if (type == typeof(uint[]))
                {
                    return new uint[0];
                }
                else if (type == typeof(long[]))
                {
                    return new long[0];
                }
                else if (type == typeof(ulong[]))
                {
                    return new ulong[0];
                }
                else if (type == typeof(char[]))
                {
                    return new char[0];
                }

                return Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object CreateListItem(Type typeList)
        {
            Type[] itemType = typeList.GetGenericArguments();
            if (itemType == null || itemType.Length == 0)
            {
                return null;
            }
            return CreateObject(itemType[0]);
        }
    }
}