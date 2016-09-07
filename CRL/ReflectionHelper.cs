﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace CRL
{


    internal static class ReflectionHelper
    {
        static Dictionary<Type, object> ReflectionInfoCache = new Dictionary<Type, object>(50);
        public static ReflectionInfo<TObject> GetInfo<TObject>()
        {
            var type = typeof(TObject);
            object info;
            if (ReflectionInfoCache.TryGetValue(type, out info))
            {
                return (ReflectionInfo<TObject>)info;
            }
            else
            {
                var refInfo = new ReflectionInfo<TObject>(type);
                ReflectionInfoCache[type] = refInfo;
                return refInfo;
            }
        }
    }

    class ReflectionInfo<TObject>
    {
        public string TableName { get; set; }

        Func<object> ObjectCreateDelegate;
        private Dictionary<string, Accessor> accessorDict;

        Func<object> CreateObjectGenerator(ConstructorInfo constructor)
        {
            Func<object> ret = null;
            ParameterInfo[] parameters = constructor.GetParameters();
            List<Expression> arguments = new List<Expression>(parameters.Length);
            var body = Expression.New(constructor, arguments);
            ret = Expression.Lambda<Func<object>>(body).Compile();
            return ret;
        }

        public ReflectionInfo(Type modelType)
        {
            var info = modelType.GetConstructor(Type.EmptyTypes);
            var act = CreateObjectGenerator(info);
            ObjectCreateDelegate = act;
            InitInfo(modelType);
            // PrimaryKey = primaryKey;
        }
        public object CreateObject()
        {
            return ObjectCreateDelegate();
        }

        private void InitInfo(Type modelType)
        {
            accessorDict = new Dictionary<string, Accessor>();
            var Properties = TypeCache.GetProperties(modelType, false);
            foreach (var kv in Properties)
            {
                Accessor accessor = null;
                var prop = kv.Value.GetPropertyInfo();
                string propName = prop.Name.ToUpper();
                var propType = prop.PropertyType;

                if (propType.IsEnum)
                {
                    propType = propType.GetEnumUnderlyingType();
                }
                if (typeof(string) == propType)
                {
                    accessor = new StringAccessor(prop);
                }
                else if (typeof(int) == propType)
                {
                    accessor = new IntAccessor(prop);
                }
                else if (typeof(int?) == propType)
                {
                    accessor = new IntNullableAccessor(prop);
                }
                else if (typeof(DateTime) == propType)
                {
                    accessor = new DateTimeAccessor(prop);
                }
                else if (typeof(DateTime?) == propType)
                {
                    accessor = new DateTimeNullableAccessor(prop);
                }
                else if (typeof(long) == propType)
                {
                    accessor = new LongAccessor(prop);
                }
                else if (typeof(long?) == propType)
                {
                    accessor = new LongNullableAccessor(prop);
                }
                else if (typeof(float) == propType)
                {
                    accessor = new FloatAccessor(prop);
                }
                else if (typeof(float?) == propType)
                {
                    accessor = new FloatNullableAccessor(prop);
                }
                else if (typeof(double) == propType)
                {
                    accessor = new DoubleAccessor(prop);
                }
                else if (typeof(double?) == propType)
                {
                    accessor = new DoubleNullableAccessor(prop);
                }
                else if (typeof(Guid) == propType)
                {
                    accessor = new GuidAccessor(prop);
                }
                else if (typeof(Guid?) == propType)
                {
                    accessor = new GuidNullableAccessor(prop);
                }
                else if (typeof(short) == propType)
                {
                    accessor = new ShortAccessor(prop);
                }
                else if (typeof(short?) == propType)
                {
                    accessor = new ShortNullableAccessor(prop);
                }
                else if (typeof(byte) == propType)
                {
                    accessor = new ByteAccessor(prop);
                }
                else if (typeof(byte?) == propType)
                {
                    accessor = new ByteNullableAccessor(prop);

                }
                else if (typeof(char) == propType)
                {
                    accessor = new CharAccessor(prop);

                }
                else if (typeof(char?) == propType)
                {
                    accessor = new CharNullableAccessor(prop);

                }
                else if (typeof(decimal) == propType)
                {
                    accessor = new DecimalAccessor(prop);

                }
                else if (typeof(decimal?) == propType)
                {
                    accessor = new DecimalNullableAccessor(prop);
                }
                else if (typeof(byte[]) == propType)
                {
                    accessor = new ByteArrayAccessor(prop);
                }
                else if (typeof(bool) == propType)
                {
                    accessor = new BoolAccessor(prop);

                }
                else if (typeof(bool?) == propType)
                {
                    accessor = new BoolNullableAccessor(prop);

                }
                else if (typeof(TimeSpan) == propType)
                {
                    accessor = new TimeSpanAccessor(prop);
                }
                else if (typeof(TimeSpan?) == propType)
                {
                    accessor = new TimeSpanNullableAccessor(prop);

                }
                accessorDict[propName] = accessor;

            }
        }

        public Accessor GetAccessor(string fieldName)
        {
            Accessor accessor;
            return accessorDict[fieldName.ToUpper()];
            if (accessorDict.TryGetValue(fieldName.ToUpper(), out accessor))
            {
                return accessor;
            }
            return null;
        }

        


        public abstract class Accessor
        {
            PropertyInfo _prop;
            public Accessor(PropertyInfo prop)
            {
                _prop = prop;
            }
            public void Set(TObject obj, object value)
            {
                if (value == null || value is DBNull)
                {
                    return;
                }
                try
                {
                    DoSet(obj, value);
                }
                catch
                {
                    throw new Exception(string.Format("无法将值{2}转换为{1},在{0}", obj.GetType(), _prop, value + " " + value.GetType()));
                }
            }

            public object Get(TObject obj)
            {
                return DoGet(obj);
            }

            protected abstract void DoSet(TObject obj, object value);
            protected abstract object DoGet(TObject obj);
        }

        #region Accessor
        public class StringAccessor : Accessor
        {
            Action<TObject, string> setter;
            Func<TObject, string> getter;

            public StringAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, string>)Delegate.CreateDelegate(typeof(Action<TObject, string>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, string>)Delegate.CreateDelegate(typeof(Func<TObject, string>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (string)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class IntAccessor : Accessor
        {
            Action<TObject, int> setter;
            Func<TObject, int> getter;
            public IntAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, int>)Delegate.CreateDelegate(typeof(Action<TObject, int>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, int>)Delegate.CreateDelegate(typeof(Func<TObject, int>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (int)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class IntNullableAccessor : Accessor
        {
            Action<TObject, int?> setter;
            Func<TObject, int?> getter;
            public IntNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, int?>)Delegate.CreateDelegate(typeof(Action<TObject, int?>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, int?>)Delegate.CreateDelegate(typeof(Func<TObject, int?>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (int)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class DateTimeAccessor : Accessor
        {
            Action<TObject, DateTime> setter;
            Func<TObject, DateTime> getter;
            public DateTimeAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, DateTime>)Delegate.CreateDelegate(typeof(Action<TObject, DateTime>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, DateTime>)Delegate.CreateDelegate(typeof(Func<TObject, DateTime>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (DateTime)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class DateTimeNullableAccessor : Accessor
        {
            Action<TObject, DateTime?> setter;
            Func<TObject, DateTime?> getter;
            public DateTimeNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, DateTime?>)Delegate.CreateDelegate(typeof(Action<TObject, DateTime?>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, DateTime?>)Delegate.CreateDelegate(typeof(Func<TObject, DateTime?>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (DateTime?)value);
            }

            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class LongAccessor : Accessor
        {
            Action<TObject, long> setter;
            Func<TObject, long> getter;
            public LongAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, long>)Delegate.CreateDelegate(typeof(Action<TObject, long>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, long>)Delegate.CreateDelegate(typeof(Func<TObject, long>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (long)value);
            }

            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class LongNullableAccessor : Accessor
        {
            Action<TObject, long?> setter;
            Func<TObject, long?> getter;
            public LongNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, long?>)Delegate.CreateDelegate(typeof(Action<TObject, long?>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, long?>)Delegate.CreateDelegate(typeof(Func<TObject, long?>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (long)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class DoubleAccessor : Accessor
        {
            Action<TObject, double> setter;
            Func<TObject, double> getter;
            public DoubleAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, double>)Delegate.CreateDelegate(typeof(Action<TObject, double>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, double>)Delegate.CreateDelegate(typeof(Func<TObject, double>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (double)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class DoubleNullableAccessor : Accessor
        {
            Action<TObject, double?> setter;
            Func<TObject, double?> getter;
            public DoubleNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, double?>)Delegate.CreateDelegate(typeof(Action<TObject, double?>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, double?>)Delegate.CreateDelegate(typeof(Func<TObject, double?>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (double)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class FloatAccessor : Accessor
        {
            Action<TObject, float> setter;
            Func<TObject, float> getter;
            public FloatAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, float>)Delegate.CreateDelegate(typeof(Action<TObject, float>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, float>)Delegate.CreateDelegate(typeof(Func<TObject, float>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (float)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class FloatNullableAccessor : Accessor
        {
            Action<TObject, float?> setter;
            Func<TObject, float?> getter;
            public FloatNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, float?>)Delegate.CreateDelegate(typeof(Action<TObject, float?>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, float?>)Delegate.CreateDelegate(typeof(Func<TObject, float?>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (float)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class GuidAccessor : Accessor
        {
            Action<TObject, Guid> setter;
            Func<TObject, Guid> getter;
            public GuidAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, Guid>)Delegate.CreateDelegate(typeof(Action<TObject, Guid>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, Guid>)Delegate.CreateDelegate(typeof(Func<TObject, Guid>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, Guid.Parse(value.ToString()));
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class GuidNullableAccessor : Accessor
        {
            Action<TObject, Guid?> setter;
            Func<TObject, Guid?> getter;
            public GuidNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, Guid?>)Delegate.CreateDelegate(typeof(Action<TObject, Guid?>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, Guid?>)Delegate.CreateDelegate(typeof(Func<TObject, Guid?>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, Guid.Parse(value.ToString()));
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class ByteAccessor : Accessor
        {
            Action<TObject, byte> setter;
            Func<TObject, byte> getter;
            public ByteAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, byte>)Delegate.CreateDelegate(typeof(Action<TObject, byte>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, byte>)Delegate.CreateDelegate(typeof(Func<TObject, byte>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, Convert.ToByte(value));
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }
        public class ByteNullableAccessor : Accessor
        {
            Action<TObject, byte?> setter;
            Func<TObject, byte?> getter;

            public ByteNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, byte?>)Delegate.CreateDelegate(typeof(Action<TObject, byte?>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, byte?>)Delegate.CreateDelegate(typeof(Func<TObject, byte?>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, Convert.ToByte(value));
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class ShortAccessor : Accessor
        {
            Action<TObject, short> setter;
            Func<TObject, short> getter;
            public ShortAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, short>)Delegate.CreateDelegate(typeof(Action<TObject, short>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, short>)Delegate.CreateDelegate(typeof(Func<TObject, short>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (short)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }
        public class ShortNullableAccessor : Accessor
        {
            Action<TObject, short?> setter;
            Func<TObject, short?> getter;
            public ShortNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, short?>)Delegate.CreateDelegate(typeof(Action<TObject, short?>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, short?>)Delegate.CreateDelegate(typeof(Func<TObject, short?>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (short)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class CharAccessor : Accessor
        {
            Action<TObject, char> setter;
            Func<TObject, char> getter;
            public CharAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, char>)Delegate.CreateDelegate(typeof(Action<TObject, char>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, char>)Delegate.CreateDelegate(typeof(Func<TObject, char>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (char)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class CharNullableAccessor : Accessor
        {
            Action<TObject, char?> setter;
            Func<TObject, char?> getter;
            public CharNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, char?>)Delegate.CreateDelegate(typeof(Action<TObject, char?>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, char?>)Delegate.CreateDelegate(typeof(Func<TObject, char?>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (char)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class BoolAccessor : Accessor
        {
            Action<TObject, bool> setter;
            Func<TObject, bool> getter;
            public BoolAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, bool>)Delegate.CreateDelegate(typeof(Action<TObject, bool>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, bool>)Delegate.CreateDelegate(typeof(Func<TObject, bool>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                bool theValue = false;
                if (value.GetType() == typeof(bool))
                {
                    theValue = (bool)value;
                }
                else
                {
                    var intValue = Convert.ToInt32(value);
                    theValue = intValue > 0;
                }
                setter(obj, theValue);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class BoolNullableAccessor : Accessor
        {
            Action<TObject, bool?> setter;
            Func<TObject, bool?> getter;
            public BoolNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, bool?>)Delegate.CreateDelegate(typeof(Action<TObject, bool?>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, bool?>)Delegate.CreateDelegate(typeof(Func<TObject, bool?>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                bool theValue = false;
                if (value.GetType() == typeof(bool))
                {
                    theValue = (bool)value;
                }
                else
                {
                    var intValue = Convert.ToInt32(value);
                    theValue = intValue > 0;
                }
                setter(obj, theValue);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class TimeSpanAccessor : Accessor
        {
            Action<TObject, TimeSpan> setter;
            Func<TObject, TimeSpan> getter;
            public TimeSpanAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, TimeSpan>)Delegate.CreateDelegate(typeof(Action<TObject, TimeSpan>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, TimeSpan>)Delegate.CreateDelegate(typeof(Func<TObject, TimeSpan>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (TimeSpan)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class TimeSpanNullableAccessor : Accessor
        {
            Action<TObject, TimeSpan?> setter;
            Func<TObject, TimeSpan?> getter;
            public TimeSpanNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, TimeSpan?>)Delegate.CreateDelegate(typeof(Action<TObject, TimeSpan?>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, TimeSpan?>)Delegate.CreateDelegate(typeof(Func<TObject, TimeSpan?>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (TimeSpan)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class DecimalAccessor : Accessor
        {
            Action<TObject, decimal> setter;
            Func<TObject, decimal> getter;
            public DecimalAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, decimal>)Delegate.CreateDelegate(typeof(Action<TObject, decimal>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, decimal>)Delegate.CreateDelegate(typeof(Func<TObject, decimal>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (decimal)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class DecimalNullableAccessor : Accessor
        {
            Action<TObject, decimal?> setter;
            Func<TObject, decimal?> getter;
            public DecimalNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, decimal?>)Delegate.CreateDelegate(typeof(Action<TObject, decimal?>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, decimal?>)Delegate.CreateDelegate(typeof(Func<TObject, decimal?>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (decimal)value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        public class ByteArrayAccessor : Accessor
        {
            Action<TObject, byte[]> setter;
            Func<TObject, byte[]> getter;
            public ByteArrayAccessor(PropertyInfo prop)
                : base(prop)
            {
                setter = (Action<TObject, byte[]>)Delegate.CreateDelegate(typeof(Action<TObject, byte[]>), null, prop.GetSetMethod(true));
                getter = (Func<TObject, byte[]>)Delegate.CreateDelegate(typeof(Func<TObject, byte[]>), null, prop.GetGetMethod(true));
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter(obj, (byte[])value);
            }
            protected override object DoGet(TObject obj)
            {
                return getter(obj);
            }
        }

        #endregion

    }


}


