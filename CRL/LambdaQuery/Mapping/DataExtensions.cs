using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery.Mapping
{
    internal class DataExtensions
    {
        static Dictionary<Type, MethodInfo> methods = new Dictionary<Type, MethodInfo>();
        public static MethodInfo GetMethod(Type propType)
        {
            if (propType.IsEnum)
            {
                propType = propType.GetEnumUnderlyingType();
            }
            if (propType.FullName.StartsWith("System.Nullable"))
            {
                //Nullable<T> 可空属性
                propType = propType.GenericTypeArguments[0];
            }
            MethodInfo result;
            var Type2 = typeof(DataExtensions);
            if (methods.Count == 0)
            {
                var array = Type2.GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach (var item in array)
                {
                    if (item.Name == "GetMethod")
                        continue;
                    methods.Add(item.ReturnType, item);
                }
            }
            var a = methods.TryGetValue(propType, out result);
            if (a)
            {
                return result;
            }
            if (propType == typeof(Guid))
            {
                result = Type2.GetMethod("GetGuid");
            }
            return result;
        }
        #region method
        public static short GetInt16(DataContainer data)
        {
            var obj = data.GetValue();
            return (short)obj;
        }
        public static int GetInt32(DataContainer data)
        {
            var obj = data.GetValue();
            return (int)obj;
        }
        public static long GetInt64(DataContainer data)
        {
            var obj = data.GetValue();
            return (long)obj;
        }

        public static decimal GetDecimal(DataContainer data)
        {
            var obj = data.GetValue();
            return (decimal)obj;
        }

        public static double GetDouble(DataContainer data)
        {
            var obj = data.GetValue();
            return (double)obj;
        }
        public static float GetFloat(DataContainer data)
        {
            var obj = data.GetValue();
            return (float)obj;
        }

        public static bool GetBoolean(DataContainer data)
        {
            var obj = data.GetValue();
            return (bool)obj;
        }
        public static DateTime GetDateTime(DataContainer data)
        {
            var obj = data.GetValue();
            return (DateTime)obj;
        }
        public static Guid GetGuid(DataContainer data)
        {
            var obj = data.GetValue();
            return (Guid)obj;
        }
        public static byte GetByte(DataContainer data)
        {
            var obj = data.GetValue();
            return (byte)obj;
        }

        public static char GetChar(DataContainer data)
        {
            var obj = data.GetValue();
            return (char)obj;
        }

        public static string GetString(DataContainer data)
        {
            var obj = data.GetValue();
            if (obj == DBNull.Value)
            {
                return null;
            }

            return (string)obj;
        }
        public static object GetValue(DataContainer data)
        {
            var obj = data.GetValue();
            if (obj == DBNull.Value)
            {
                return null;
            }

            return obj;
        }
        #endregion
    }
}
