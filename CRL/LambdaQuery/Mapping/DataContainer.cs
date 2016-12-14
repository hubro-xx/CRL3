using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery.Mapping
{
    public class DataContainer
    {
        object[] data;
        int index = 0;
        Type dataType;
        public DataContainer(object[] _data, Type _dataType)
        {
            dataType = _dataType;
            data = _data;
        }
        public T _GetValue<T>()
        {
            var result = data.GetValue(index);
            index++;
            if (result is DBNull)
            {
                return default(T);
            }
            try
            {
                return (T)result;
            }
            catch
            {
                throw new CRLException(string.Format("将值 {0} 转换成类型{1}.{2}时失败,请检查对象类型和数据表字段类型是否一致", result + " " + typeof(T), dataType));
            }
        }
        T _GetIndexValue<T>(int _index)
        {
            var result = data.GetValue(_index);
            if (result is DBNull)
            {
                return default(T);
            }
            try
            {
                return (T)result;
            }
            catch
            {
                throw new CRLException(string.Format("将值 {0} 转换成类型{1}.{2}时失败,请检查对象类型和数据表字段类型是否一致", result + " " + typeof(T), dataType));
            }
        }
        #region method
        public short GetInt16()
        {
            var obj = _GetValue<short>();
            return obj;
        }
        public short? GetInt16Nullable()
        {
            var obj = _GetValue<short>();
            return obj;
        }
        public int GetInt32()
        {
            var obj = _GetValue<int>();
            return obj;
        }
        public int? GetInt32Nullable()
        {
            var obj = _GetValue<int>();
            return obj;
        }
        public long GetInt64()
        {
            var obj = _GetValue<long>();
            return obj;
        }
        public long? GetInt64Nullable()
        {
            var obj = _GetValue<long>();
            return obj;
        }
        public decimal GetDecimal()
        {
            var obj = _GetValue<decimal>();
            return obj;
        }
        public decimal? GetDecimalNullable()
        {
            var obj = _GetValue<decimal>();
            return obj;
        }
        public double GetDouble()
        {
            var obj = _GetValue<double>();
            return obj;
        }
        public double? GetDoubleNullable()
        {
            var obj = _GetValue<double>();
            return obj;
        }
        public float GetFloat()
        {
            var obj = _GetValue<float>();
            return obj;
        }
        public float? GetFloatNullable()
        {
            var obj = _GetValue<float>();
            return obj;
        }
        public bool GetBoolean()
        {
            var obj = _GetValue<bool>();
            return obj;
        }
        public bool? GetBooleanNullable()
        {
            var obj = _GetValue<bool>();
            return obj;
        }
        public DateTime GetDateTime()
        {
            var obj = _GetValue<DateTime>();
            return obj;
        }
        public DateTime? GetDateTimeNullable()
        {
            var obj = _GetValue<DateTime>();
            return obj;
        }
        public Guid GetGuid()
        {
            var obj = _GetValue<Guid>();
            return obj;
        }
        public Guid? GetGuidNullable()
        {
            var obj = _GetValue<Guid>();
            return obj;
        }
        public byte GetByte()
        {
            var obj = _GetValue<byte>();
            return obj;
        }
        public byte? GetByteNullable()
        {
            var obj = _GetValue<byte>();
            return obj;
        }
        public char GetChar()
        {
            var obj = _GetValue<char>();
            return obj;
        }
        public char? GetCharNullable()
        {
            var obj = _GetValue<char>();
            return obj;
        }
        public string GetString()
        {
            var obj = _GetValue<string>();
            return obj;
        }
        public object GetIndexValue()
        {
            var obj = _GetValue<object>();
            return obj;
        }
        #endregion
        static Dictionary<Type, MethodInfo> methods = new Dictionary<Type, MethodInfo>();
        public static MethodInfo GetMethod(Type propType)
        {
            if (propType.IsEnum)
            {
                propType = propType.GetEnumUnderlyingType();
            }
            MethodInfo result;
            var Type2 = typeof(DataContainer);
            if (methods.Count == 0)
            {
                var array = Type2.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (var item in array)
                {
                    if (item.Name == "GetHashCode")
                    {
                        continue;
                    }
                    if (!item.Name.StartsWith("Get"))
                    {
                        continue;
                    }
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
    }
}
