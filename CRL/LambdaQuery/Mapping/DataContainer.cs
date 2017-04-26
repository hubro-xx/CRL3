using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery.Mapping
{
    class DataContainer1
    {
        object[] data;
        int index = 0;
        Type dataType;
        public DataContainer1(object[] _data, Type _dataType)
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
            var Type2 = typeof(DataContainer1);
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

    class DataContainer
    {
        DbDataReader reader;
        Type dataType;
        Dictionary<string, int> columnMapping;
        internal int currentDataIndex = 0;
        public DataContainer(DbDataReader _reader, Type _dataType, Dictionary<string, int> _columnMapping)
        {
            dataType = _dataType;
            reader = _reader;
            columnMapping = _columnMapping;
        }
        internal string _GetCurrentColumnName()
        {
            foreach (var kv in columnMapping)
            {
                if (kv.Value == currentDataIndex)
                {
                    return kv.Key;
                }
            }
            return "";
        }
        T _GetValue<T>(int _index)
        {
            var result = reader.GetValue(_index);
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
        public TEnum GetEnum<TEnum>(int index) where TEnum : struct
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(TEnum);
            }
            var value = reader.GetInt32(index);
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }
        public TEnum? GetEnumNullable<TEnum>(int index) where TEnum : struct
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(TEnum);
            }
            int value = reader.GetInt32(index);
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        public short GetInt16(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(short);
            }
            return reader.GetInt16(index);
        }
        public short? GetInt16Nullable(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetInt16(index);
        }
        public int GetInt32(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(int);
            }
            return reader.GetInt32(index);
        }
        public int? GetInt32Nullable(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetInt32(index);
        }
        public long GetInt64(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(long);
            }
            return reader.GetInt64(index);
        }
        public long? GetInt64Nullable(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetInt64(index);
        }
        public decimal GetDecimal(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(decimal);
            }
            return reader.GetDecimal(index);
        }
        public decimal? GetDecimalNullable(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetDecimal(index);
        }
        public double GetDouble(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(double);
            }
            return reader.GetDouble(index);
        }
        public double? GetDoubleNullable(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetDouble(index);
        }
        public float GetFloat(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(float);
            }
            return reader.GetFloat(index);
        }
        public float? GetFloatNullable(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetFloat(index);
        }
        public bool GetBoolean(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(bool);
            }
            return reader.GetBoolean(index);
        }
        public bool? GetBooleanNullable(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetBoolean(index);
        }
        public DateTime GetDateTime(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(DateTime);
            }
            return reader.GetDateTime(index);
        }
        public DateTime? GetDateTimeNullable(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetDateTime(index);
        }
        public Guid GetGuid(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(Guid);
            }
            return reader.GetGuid(index);
        }
        public Guid? GetGuidNullable(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetGuid(index);
        }
        public byte GetByte(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(byte);
            }
            return reader.GetByte(index);
        }
        public byte? GetByteNullable(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetByte(index);
        }
        public char GetChar(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return default(char);
            }
            return reader.GetChar(index);
        }
        public char? GetCharNullable(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetChar(index);
        }
        public string GetString(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetString(index);
        }
        public object GetIndexValue(int index)
        {
            currentDataIndex = index;
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetValue(index);
        }
        #endregion
        static Dictionary<Type, MethodInfo> methods = new Dictionary<Type, MethodInfo>();
        public static MethodInfo GetMethod(Type propType, bool anonymousClass = false)
        {
            var  unType = Nullable.GetUnderlyingType(propType);
            var isNullable = unType != null;
            MethodInfo result;
            if (propType.IsEnum && !anonymousClass)//按是按lanbda表达式便建对象赋值时,需返回强类型方法
            {
                propType = propType.GetEnumUnderlyingType();
            }
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
            if (propType.IsEnum && anonymousClass)
            {
                //按是按lanbda表达式便建对象赋值时,需返回强类型方法
                var m1 = Type2.GetMethod("GetEnum");
                var m2 = Type2.GetMethod("GetEnumNullable");
                if (isNullable)
                {
                    return m2.MakeGenericMethod(unType);
                }
                return m1.MakeGenericMethod(propType);
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
