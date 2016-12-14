using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery.Mapping
{
    public class DataExtensions
    {
        static Dictionary<Type, MethodInfo> methods = new Dictionary<Type, MethodInfo>();
        public static MethodInfo GetMethod(Type propType)
        {
            if (propType.IsEnum)
            {
                propType = propType.GetEnumUnderlyingType();
            }
            //if (propType.FullName.StartsWith("System.Nullable"))
            //{
            //    //Nullable<T> 可空属性
            //    propType = propType.GenericTypeArguments[0];
            //}
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
            var obj = data._GetValue<short>();
            return obj;
        }
        public static short? GetInt16Nullable(DataContainer data)
        {
            var obj = data._GetValue<short>();
            return obj;
        }
        public static int GetInt32(DataContainer data)
        {
            var obj = data._GetValue<int>();
            return obj;
        }
        public static int? GetInt32Nullable(DataContainer data)
        {
            var obj = data._GetValue<int>();
            return obj;
        }
        public static long GetInt64(DataContainer data)
        {
            var obj = data._GetValue<long>();
            return obj;
        }
        public static long? GetInt64Nullable(DataContainer data)
        {
            var obj = data._GetValue<long>();
            return obj;
        }
        public static decimal GetDecimal(DataContainer data)
        {
            var obj = data._GetValue<decimal>();
            return obj;
        }
        public static decimal? GetDecimalNullable(DataContainer data)
        {
            var obj = data._GetValue<decimal>();
            return obj;
        }
        public static double GetDouble(DataContainer data)
        {
            var obj = data._GetValue<double>();
            return obj;
        }
        public static double? GetDoubleNullable(DataContainer data)
        {
            var obj = data._GetValue<double>();
            return obj;
        }
        public static float GetFloat(DataContainer data)
        {
            var obj = data._GetValue<float>();
            return obj;
        }
        public static float? GetFloatNullable(DataContainer data)
        {
            var obj = data._GetValue<float>();
            return obj;
        }
        public static bool GetBoolean(DataContainer data)
        {
            var obj = data._GetValue<bool>();
            return obj;
        }
        public static bool? GetBooleanNullable(DataContainer data)
        {
            var obj = data._GetValue<bool>();
            return obj;
        }
        public static DateTime GetDateTime(DataContainer data)
        {
            var obj = data._GetValue<DateTime>();
            return obj;
        }
        public static DateTime? GetDateTimeNullable(DataContainer data)
        {
            var obj = data._GetValue<DateTime>();
            return obj;
        }
        public static Guid GetGuid(DataContainer data)
        {
            var obj = data._GetValue<Guid>();
            return obj;
        }
        public static Guid? GetGuidNullable(DataContainer data)
        {
            var obj = data._GetValue<Guid>();
            return obj;
        }
        public static byte GetByte(DataContainer data)
        {
            var obj = data._GetValue<byte>();
            return obj;
        }
        public static byte? GetByteNullable(DataContainer data)
        {
            var obj = data._GetValue<byte>();
            return obj;
        }
        public static char GetChar(DataContainer data)
        {
            var obj = data._GetValue<char>();
            return obj;
        }
        public static char? GetCharNullable(DataContainer data)
        {
            var obj = data._GetValue<char>();
            return obj;
        }
        public static string GetString(DataContainer data)
        {
            var obj = data._GetValue<string>();
            return obj;
        }
        public static object GetValue(DataContainer data)
        {
            var obj = data._GetValue<object>();
            return obj;
        }
        #endregion
    }
}
