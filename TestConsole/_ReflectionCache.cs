using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    static class ReflectionCache
    {
        static Dictionary<Type, string> TableNamesDict = new Dictionary<Type, string>();
        static Dictionary<string, Tuple<PropertyInfo, Delegate>> PPDict = new Dictionary<string, Tuple<PropertyInfo, Delegate>>();

        static Dictionary<Type, PropertyInfo[]> PropertysDict = new Dictionary<Type, PropertyInfo[]>();
        static Dictionary<MemberInfo, object[]> CustomAttributesDict = new Dictionary<MemberInfo, object[]>();
        static Dictionary<string, Delegate> SetterDict = new Dictionary<string, Delegate>();


        public static PropertyInfo[] GetCachedProperties(this Type type)
        {
            PropertyInfo[] value;
            if (PropertysDict.TryGetValue(type, out value))
            {
                return value;
            }
            value = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertysDict[type] = value;
            return value;
        }

        public static Tuple<PropertyInfo, Delegate> GetCachedPP<TObject>(this Type type, string propertyName)
        {

            var key = (type.FullName + "." + propertyName).ToUpper();
            Tuple<PropertyInfo, Delegate> tuple = null;
            if (PPDict.TryGetValue(key, out tuple))
            {
                return tuple;
            }
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var seted = false;
            foreach (var prop in properties)
            {
                Delegate action = null;
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
                else
                {
                    action = Delegate.CreateDelegate(typeof(Action<TObject, object>), null, prop.GetSetMethod());
                }
                var val = new Tuple<PropertyInfo, Delegate>(prop, action);

                if (!seted && prop.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    tuple = val;
                    seted = true;
                }
                else
                {
                    PPDict[(type.FullName + "." + prop.Name).ToUpper()] = val;
                }
            }
            PPDict[key] = tuple;
            return tuple;
        }

        public static object[] GetCachedCustomAttributes(this MemberInfo member, Type attributeType)
        {
            object[] value;
            if (CustomAttributesDict.TryGetValue(member, out value))
            {
                return value;
            }
            value = member.GetCustomAttributes(attributeType, false);
            CustomAttributesDict[member] = value;
            return value;
        }
    }
}
