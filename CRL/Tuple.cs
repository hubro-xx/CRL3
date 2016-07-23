using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL
{
    internal class Tuple
    {
        static Dictionary<string, Tuple<PropertyInfo, Delegate>> TupleDelegateCache1 = new Dictionary<string, Tuple<PropertyInfo, Delegate>>();
        static Dictionary<string, DelegateCacheItem> DelegateCache = new Dictionary<string, DelegateCacheItem>();
        internal class DelegateCacheItem
        {
            public void InIt<T1, T2>(PropertyInfo prop)
            {
                var action = Delegate.CreateDelegate(typeof(Action<T1, T2>), null, prop.GetSetMethod());
                //TupleDelegate = new Tuple<PropertyInfo, Delegate>(prop, action);
                SetValue = (a, b) =>
                {
                    ((Action<T1, T2>)action)((T1)a, (T2)b);
                };
            }
            //public Tuple<PropertyInfo, Delegate> TupleDelegate;
            public Action<object, object> SetValue;
        }
        internal static DelegateCacheItem GetCacheDelegate<T>(Type type, string propertyName)
        {
            var key = (type.FullName + "." + propertyName).ToUpper();
            DelegateCacheItem deletageItem = null;
            if (DelegateCache.TryGetValue(key, out deletageItem))
            {
                return deletageItem;
            }
            var properties = TypeCache.GetProperties(type, false);
            foreach (var item in properties)
            {
                var prop = item.Value.GetPropertyInfo();
                var cacheItem = new DelegateCacheItem();
                #region 判断
                if (typeof(int) == prop.PropertyType)
                {
                    cacheItem.InIt<T, int>(prop);
                }
                else if (typeof(int?) == prop.PropertyType)
                {
                    cacheItem.InIt<T, int?>(prop);
                }
                else if (typeof(string) == prop.PropertyType)
                {
                    cacheItem.InIt<T, string>(prop);
                }
                else if (typeof(DateTime) == prop.PropertyType)
                {
                    cacheItem.InIt<T, DateTime>(prop);
                }
                else if (typeof(DateTime?) == prop.PropertyType)
                {
                    cacheItem.InIt<T, DateTime?>(prop);
                }
                else if (typeof(long) == prop.PropertyType)
                {
                    cacheItem.InIt<T, long>(prop);
                }
                else if (typeof(long?) == prop.PropertyType)
                {
                    cacheItem.InIt<T, long?>(prop);
                }
                else if (typeof(float) == prop.PropertyType)
                {
                    cacheItem.InIt<T, float>(prop);
                }
                else if (typeof(float?) == prop.PropertyType)
                {
                    cacheItem.InIt<T, float?>(prop);
                }
                else if (typeof(double) == prop.PropertyType)
                {
                    cacheItem.InIt<T, double>(prop);
                }
                else if (typeof(double?) == prop.PropertyType)
                {
                    cacheItem.InIt<T, double?>(prop);
                }
                else if (typeof(Guid) == prop.PropertyType)
                {
                    cacheItem.InIt<T, Guid>(prop);
                }
                else if (typeof(Guid?) == prop.PropertyType)
                {
                    cacheItem.InIt<T, Guid?>(prop);
                }
                else if (typeof(short) == prop.PropertyType)
                {
                    cacheItem.InIt<T, short>(prop);
                }
                else if (typeof(short?) == prop.PropertyType)
                {
                    cacheItem.InIt<T, short?>(prop);
                }
                else if (typeof(byte) == prop.PropertyType)
                {
                    cacheItem.InIt<T, byte>(prop);
                }
                else if (typeof(byte?) == prop.PropertyType)
                {
                    cacheItem.InIt<T, byte?>(prop);
                }
                else if (typeof(char) == prop.PropertyType)
                {
                    cacheItem.InIt<T, char>(prop);
                }
                else if (typeof(char?) == prop.PropertyType)
                {
                    cacheItem.InIt<T, char?>(prop);
                }
                else if (typeof(decimal) == prop.PropertyType)
                {
                    cacheItem.InIt<T, decimal>(prop);
                }
                else if (typeof(decimal?) == prop.PropertyType)
                {
                    cacheItem.InIt<T, decimal?>(prop);
                }
                else if (typeof(byte[]) == prop.PropertyType)
                {
                    cacheItem.InIt<T, byte[]>(prop);
                }
                else if (typeof(bool) == prop.PropertyType)
                {
                    cacheItem.InIt<T, bool>(prop);
                }
                else if (typeof(bool?) == prop.PropertyType)
                {
                    cacheItem.InIt<T, bool?>(prop);
                }
                else if (typeof(TimeSpan?) == prop.PropertyType)
                {
                    cacheItem.InIt<T, TimeSpan?>(prop);
                }
                else if (typeof(TimeSpan) == prop.PropertyType)
                {
                    cacheItem.InIt<T, TimeSpan>(prop);
                }
                else if (typeof(Enum) == prop.PropertyType.BaseType)
                {
                    cacheItem.InIt<T, int>(prop);
                }
                else
                {
                    cacheItem.InIt<T, object>(prop);
                }
                #endregion
                DelegateCache[(type.FullName + "." + prop.Name).ToUpper()] = cacheItem;
            }
            return DelegateCache[key];
        }
        #region old
        internal static Tuple<PropertyInfo, Delegate> GetCacheTupleDelegate1<T>(Type type, string propertyName)
        {
            var key = (type.FullName + "." + propertyName).ToUpper();
            Tuple<PropertyInfo, Delegate> tuple = null;
            if (TupleDelegateCache1.TryGetValue(key, out tuple))
            {
                return tuple;
            }
            var properties = TypeCache.GetProperties(type, false);
            foreach (var item in properties)
            {
                Delegate action = null;
                var prop = item.Value.GetPropertyInfo();
                var method = prop.GetSetMethod();
                Type delegateType = null;
                #region 判断
                if (typeof(int) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, int>);
                }
                else if (typeof(int?) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, int?>);
                }
                else if (typeof(string) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, string>);
                }
                else if (typeof(DateTime) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, DateTime>);
                }
                else if (typeof(DateTime?) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, DateTime?>);
                }
                else if (typeof(long) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, long>);
                }
                else if (typeof(long?) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, long?>);
                }
                else if (typeof(float) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, float>);
                }
                else if (typeof(float?) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, float?>);
                }
                else if (typeof(double) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, double>);
                }
                else if (typeof(double?) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, double?>);
                }
                else if (typeof(Guid) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, Guid>);
                }
                else if (typeof(Guid?) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, Guid?>);
                }
                else if (typeof(short) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, short>);
                }
                else if (typeof(short?) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, short?>);
                }
                else if (typeof(byte) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, byte>);
                }
                else if (typeof(byte?) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, byte?>);
                }
                else if (typeof(char) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, char>);
                }
                else if (typeof(char?) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, char?>);
                }
                else if (typeof(decimal) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, decimal>);
                }
                else if (typeof(decimal?) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, decimal?>);
                }
                else if (typeof(byte[]) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, byte[]>);
                }
                else if (typeof(bool) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, bool>);
                }
                else if (typeof(bool?) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, bool?>);
                }
                else if (typeof(TimeSpan?) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, TimeSpan?>);
                }
                else if (typeof(TimeSpan) == prop.PropertyType)
                {
                    delegateType = typeof(Action<T, TimeSpan>);
                }
                else if (typeof(Enum) == prop.PropertyType.BaseType)
                {
                    delegateType = typeof(Action<T, int>);
                }
                else
                {
                    delegateType = typeof(Action<T, object>);
                }
                #endregion
                action = Delegate.CreateDelegate(delegateType, null, method);
                var val = new Tuple<PropertyInfo, Delegate>(prop, action);
                TupleDelegateCache1[(type.FullName + "." + prop.Name).ToUpper()] = val;
            }
            return TupleDelegateCache1[key];
        }

        internal static void SetPropertyValue1<T>(Tuple<PropertyInfo, Delegate> tuple, T obj, object value)
        {
            var fieldType = tuple.Item1.PropertyType;
            //((Action<T, object>)tuple.Item2)(obj, value);
            //return;
            if (fieldType == typeof(string))
            {
                ((Action<T, string>)tuple.Item2)(obj, (string)value);
            }
            else if (fieldType == typeof(int))
            {
                ((Action<T, int>)tuple.Item2)(obj, Convert.ToInt32(value));
            }
            else if (fieldType == typeof(int?))
            {
                ((Action<T, int?>)tuple.Item2)(obj, (int)value);
            }
            else if (fieldType == typeof(DateTime))
            {
                ((Action<T, DateTime>)tuple.Item2)(obj, (DateTime)value);
            }
            else if (fieldType == typeof(DateTime?))
            {
                ((Action<T, DateTime?>)tuple.Item2)(obj, (DateTime)value);
            }
            else if (fieldType == typeof(long))
            {
                ((Action<T, long>)tuple.Item2)(obj, (long)value);
            }
            else if (fieldType == typeof(long?))
            {
                ((Action<T, long?>)tuple.Item2)(obj, (long)value);
            }
            else if (fieldType == typeof(double))
            {
                ((Action<T, double>)tuple.Item2)(obj, (double)value);
            }
            else if (fieldType == typeof(double?))
            {
                ((Action<T, double?>)tuple.Item2)(obj, (double)value);
            }
            else if (fieldType == typeof(Guid))
            {
                ((Action<T, Guid>)tuple.Item2)(obj, (Guid)value);
            }
            else if (fieldType == typeof(Guid?))
            {
                ((Action<T, Guid?>)tuple.Item2)(obj, (Guid)value);
            }
            else if (fieldType == typeof(float))
            {
                ((Action<T, float>)tuple.Item2)(obj, (float)value);
            }
            else if (fieldType == typeof(float?))
            {
                ((Action<T, float?>)tuple.Item2)(obj, (float)value);
            }
            else if (fieldType == typeof(byte))
            {
                ((Action<T, byte>)tuple.Item2)(obj, Convert.ToByte(value));
            }
            else if (fieldType == typeof(byte?))
            {
                ((Action<T, byte?>)tuple.Item2)(obj, (byte)value);
            }
            else if (fieldType == typeof(char))
            {
                ((Action<T, char>)tuple.Item2)(obj, (char)value);
            }
            else if (fieldType == typeof(char?))
            {
                ((Action<T, char?>)tuple.Item2)(obj, (char)value);
            }
            else if (fieldType == typeof(bool))
            {
                bool theValue = false;
                if (value is bool)
                {
                    theValue = (bool)value;
                }
                else
                {
                    var intValue = Convert.ToInt32(value);
                    theValue = intValue > 0;
                }
                ((Action<T, bool>)tuple.Item2)(obj, theValue);
            }
            else if (fieldType == typeof(bool?))
            {
                ((Action<T, bool?>)tuple.Item2)(obj, (bool)value);
            }
            else if (fieldType == typeof(decimal))
            {
                ((Action<T, decimal>)tuple.Item2)(obj, (decimal)value);
            }
            else if (fieldType.BaseType == typeof(Enum))
            {
                ((Action<T, int>)tuple.Item2)(obj, (int)value);
            }
            else if (fieldType == typeof(decimal?))
            {
                ((Action<T, decimal?>)tuple.Item2)(obj, (decimal)value);
            }
            else if (value is byte[])
            {
                ((Action<T, byte[]>)tuple.Item2)(obj, (byte[])value);
            }
        }
        #endregion
    }
}
