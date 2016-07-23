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
        static Dictionary<string, Tuple<PropertyInfo, Delegate>> PPDict = new Dictionary<string, Tuple<PropertyInfo, Delegate>>();
        internal static Tuple<PropertyInfo, Delegate> GetCachedPP<TObject>(Type type, string propertyName)
        {
            var key = (type.FullName + "." + propertyName).ToUpper();
            Tuple<PropertyInfo, Delegate> tuple = null;
            if (PPDict.TryGetValue(key, out tuple))
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
                    action = Delegate.CreateDelegate(typeof(Action<TObject, int>), null, prop.GetSetMethod());
                }
                else if (typeof(int?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, int?>), null, prop.GetSetMethod());
                }
                else if (typeof(string) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, string>), null, prop.GetSetMethod());
                }
                else if (typeof(DateTime) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, DateTime>), null, prop.GetSetMethod());
                }
                else if (typeof(DateTime?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, DateTime?>), null, prop.GetSetMethod());
                }

                else if (typeof(long) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, long>), null, prop.GetSetMethod());
                }
                else if (typeof(long?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, long?>), null, prop.GetSetMethod());
                }
                else if (typeof(float) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, float>), null, prop.GetSetMethod());
                }
                else if (typeof(float?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, float?>), null, prop.GetSetMethod());
                }
                else if (typeof(double) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, double>), null, prop.GetSetMethod());
                }
                else if (typeof(double?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, double?>), null, prop.GetSetMethod());
                }
                else if (typeof(Guid) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, Guid>), null, prop.GetSetMethod());
                }
                else if (typeof(Guid?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, Guid?>), null, prop.GetSetMethod());
                }
                else if (typeof(short) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, short>), null, prop.GetSetMethod());
                }
                else if (typeof(short?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, short?>), null, prop.GetSetMethod());
                }
                else if (typeof(byte) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, byte>), null, prop.GetSetMethod());
                }
                else if (typeof(byte?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, byte?>), null, prop.GetSetMethod());
                }
                else if (typeof(char) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, char>), null, prop.GetSetMethod());
                }
                else if (typeof(char?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, char?>), null, prop.GetSetMethod());
                }
                else if (typeof(decimal) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, decimal>), null, prop.GetSetMethod());
                }
                else if (typeof(decimal?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, decimal?>), null, prop.GetSetMethod());
                }
                else if (typeof(byte[]) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, byte[]>), null, prop.GetSetMethod());
                }
                else if (typeof(bool) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, bool>), null, prop.GetSetMethod());
                }
                else if (typeof(bool?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, bool?>), null, prop.GetSetMethod());
                }
                else if (typeof(TimeSpan?) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, TimeSpan?>), null, prop.GetSetMethod());
                }
                else if (typeof(TimeSpan) == prop.PropertyType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, TimeSpan>), null, prop.GetSetMethod());
                }
                else if (typeof(Enum) == prop.PropertyType.BaseType)
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, int>), null, prop.GetSetMethod());
                }
                else
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, object>), null, prop.GetSetMethod());
                }
                var val = new Tuple<PropertyInfo, Delegate>(prop, action);

                PPDict[(type.FullName + "." + prop.Name).ToUpper()] = val;
            }
            return PPDict[key];
        }

        internal static void SetPropertyValue<TObject>(Tuple<PropertyInfo, Delegate> tuple, Type objType, TObject obj, string fieldName, object value)
        {
            var fieldType = tuple.Item1.PropertyType;

            if (fieldType == typeof(string))
            {
                ((Action<TObject, string>)tuple.Item2)(obj, (string)value);
            }
            else if (fieldType == typeof(int))
            {
                ((Action<TObject, int>)tuple.Item2)(obj, Convert.ToInt32(value));
            }
            else if (fieldType == typeof(int?))
            {
                ((Action<TObject, int?>)tuple.Item2)(obj, (int)value);
            }
            else if (fieldType == typeof(DateTime))
            {
                ((Action<TObject, DateTime>)tuple.Item2)(obj, (DateTime)value);
            }
            else if (fieldType == typeof(DateTime?))
            {
                ((Action<TObject, DateTime?>)tuple.Item2)(obj, (DateTime)value);
            }
            else if (fieldType == typeof(long))
            {
                ((Action<TObject, long>)tuple.Item2)(obj, (long)value);
            }
            else if (fieldType == typeof(long?))
            {
                ((Action<TObject, long?>)tuple.Item2)(obj, (long)value);
            }
            else if (fieldType == typeof(double))
            {
                ((Action<TObject, double>)tuple.Item2)(obj, (double)value);
            }
            else if (fieldType == typeof(double?))
            {
                ((Action<TObject, double?>)tuple.Item2)(obj, (double)value);
            }
            else if (fieldType == typeof(Guid))
            {
                ((Action<TObject, Guid>)tuple.Item2)(obj, (Guid)value);
            }
            else if (fieldType == typeof(Guid?))
            {
                ((Action<TObject, Guid?>)tuple.Item2)(obj, (Guid)value);
            }
            else if (fieldType == typeof(float))
            {
                ((Action<TObject, float>)tuple.Item2)(obj, (float)value);
            }
            else if (fieldType == typeof(float?))
            {
                ((Action<TObject, float?>)tuple.Item2)(obj, (float)value);
            }
            else if (fieldType == typeof(byte))
            {
                ((Action<TObject, byte>)tuple.Item2)(obj, Convert.ToByte(value));
            }
            else if (fieldType == typeof(byte?))
            {
                ((Action<TObject, byte?>)tuple.Item2)(obj, (byte)value);
            }
            else if (fieldType == typeof(char))
            {
                ((Action<TObject, char>)tuple.Item2)(obj, (char)value);
            }
            else if (fieldType == typeof(char?))
            {
                ((Action<TObject, char?>)tuple.Item2)(obj, (char)value);
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
                ((Action<TObject, bool>)tuple.Item2)(obj, theValue);
            }
            else if (fieldType == typeof(bool?))
            {
                ((Action<TObject, bool?>)tuple.Item2)(obj, (bool)value);
            }
            else if (fieldType == typeof(decimal))
            {
                ((Action<TObject, decimal>)tuple.Item2)(obj, (decimal)value);
            }
            else if (fieldType == typeof(decimal?))
            {
                ((Action<TObject, decimal?>)tuple.Item2)(obj, (decimal)value);
            }
            else if (value is byte[])
            {
                ((Action<TObject, byte[]>)tuple.Item2)(obj, (byte[])value);
            }
        }

    }
}
