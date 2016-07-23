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
        static Dictionary<string, Tuple<PropertyInfo, Delegate>> TupleDelegateCache = new Dictionary<string, Tuple<PropertyInfo, Delegate>>();
        internal static Tuple<PropertyInfo, Delegate> GetCacheTupleDelegate<T>(Type type, string propertyName)
        {
            var key = (type.FullName + "." + propertyName).ToUpper();
            Tuple<PropertyInfo, Delegate> tuple = null;
            if (TupleDelegateCache.TryGetValue(key, out tuple))
            {
                return tuple;
            }
            var properties = TypeCache.GetProperties(type, false);
            foreach (var item in properties)
            {
                Delegate action = null;
                var prop = item.Value.GetPropertyInfo();
                if (typeof(int) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, int>), null, prop.GetSetMethod());
                }
                else if (typeof(int?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, int?>), null, prop.GetSetMethod());
                }
                else if (typeof(string) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, string>), null, prop.GetSetMethod());
                }
                else if (typeof(DateTime) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, DateTime>), null, prop.GetSetMethod());
                }
                else if (typeof(DateTime?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, DateTime?>), null, prop.GetSetMethod());
                }

                else if (typeof(long) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, long>), null, prop.GetSetMethod());
                }
                else if (typeof(long?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, long?>), null, prop.GetSetMethod());
                }
                else if (typeof(float) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, float>), null, prop.GetSetMethod());
                }
                else if (typeof(float?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, float?>), null, prop.GetSetMethod());
                }
                else if (typeof(double) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, double>), null, prop.GetSetMethod());
                }
                else if (typeof(double?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, double?>), null, prop.GetSetMethod());
                }
                else if (typeof(Guid) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, Guid>), null, prop.GetSetMethod());
                }
                else if (typeof(Guid?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, Guid?>), null, prop.GetSetMethod());
                }
                else if (typeof(short) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, short>), null, prop.GetSetMethod());
                }
                else if (typeof(short?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, short?>), null, prop.GetSetMethod());
                }
                else if (typeof(byte) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, byte>), null, prop.GetSetMethod());
                }
                else if (typeof(byte?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, byte?>), null, prop.GetSetMethod());
                }
                else if (typeof(char) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, char>), null, prop.GetSetMethod());
                }
                else if (typeof(char?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, char?>), null, prop.GetSetMethod());
                }
                else if (typeof(decimal) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, decimal>), null, prop.GetSetMethod());
                }
                else if (typeof(decimal?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, decimal?>), null, prop.GetSetMethod());
                }
                else if (typeof(byte[]) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, byte[]>), null, prop.GetSetMethod());
                }
                else if (typeof(bool) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, bool>), null, prop.GetSetMethod());
                }
                else if (typeof(bool?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, bool?>), null, prop.GetSetMethod());
                }
                else if (typeof(TimeSpan?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, TimeSpan?>), null, prop.GetSetMethod());
                }
                else if (typeof(TimeSpan) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, TimeSpan>), null, prop.GetSetMethod());
                }
                else if (typeof(Enum) == prop.PropertyType.BaseType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, int>), null, prop.GetSetMethod());
                }
                else
                {
                    action = Delegate.CreateDelegate(typeof(Action<T, object>), null, prop.GetSetMethod());
                }
                var val = new Tuple<PropertyInfo, Delegate>(prop, action);

                TupleDelegateCache[(type.FullName + "." + prop.Name).ToUpper()] = val;
            }
            return TupleDelegateCache[key];
        }

        internal static void SetPropertyValue<T>(Tuple<PropertyInfo, Delegate> tuple, T obj, string fieldName, object value)
        {
            var fieldType = tuple.Item1.PropertyType;

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
            else if (fieldType == typeof(decimal?))
            {
                ((Action<T, decimal?>)tuple.Item2)(obj, (decimal)value);
            }
            else if (value is byte[])
            {
                ((Action<T, byte[]>)tuple.Item2)(obj, (byte[])value);
            }
        }

    }
}
