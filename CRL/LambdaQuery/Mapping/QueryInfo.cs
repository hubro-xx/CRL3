using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery.Mapping
{
    internal class QueryInfo<TSource>
    {
        static System.Collections.Concurrent.ConcurrentDictionary<Type, Delegate> DelegateCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, Delegate>();
        public QueryInfo(bool anonymousClass, IEnumerable<Attribute.FieldMapping> mapping = null, ConstructorInfo constructor = null)
        {
            mapping = mapping ?? new List<Attribute.FieldMapping>();
            Mapping = mapping;
            AnonymousClass = anonymousClass;
            if (anonymousClass)
            {
                Delegate dg;
                //缓存处理
                var a = DelegateCache.TryGetValue(typeof(TSource), out dg);
                if (a)
                {
                    ObjCreater = (Func<DataContainer, TSource>)dg;
                    return;
                }
                ObjCreater = CreateObjectGenerator<TSource>(constructor);
                DelegateCache.TryAdd(typeof(TSource), ObjCreater);
            }
            else
            {
                Reflection = ReflectionHelper.GetInfo<TSource>();
            }

            //if (!anonymousClass)
            //{
            //    Reflection = ReflectionHelper.GetInfo<TSource>();
            //} 
            //Delegate dg;
            ////缓存处理
            //var a = DelegateCache.TryGetValue(typeof(TSource), out dg);
            //if (a)
            //{
            //    ObjCreater = (Func<DataContainer, TSource>)dg;
            //    return;
            //}
            //else
            //{
            //    if (anonymousClass)
            //    {
            //        ObjCreater = CreateObjectGenerator<TSource>(constructor);
            //    }
            //    else
            //    {
            //        ObjCreater = CreateObjectGenerator2<TSource>(mapping);
            //    }
            //    DelegateCache.TryAdd(typeof(TSource), ObjCreater);
            //}
        }
        public bool AnonymousClass;
        public IEnumerable<Attribute.FieldMapping> Mapping;
        public Func<DataContainer, TSource> ObjCreater;
        public ReflectionInfo<TSource> Reflection;

        static Func<DataContainer, T> CreateObjectGenerator<T>(ConstructorInfo constructor)
        {
            var parame = Expression.Parameter(typeof(DataContainer), "par");
            ParameterInfo[] parameters = constructor.GetParameters();
            List<Expression> arguments = new List<Expression>(parameters.Length);
            foreach (var parameter in parameters)
            {
                var method = DataExtensions.GetMethod(parameter.ParameterType);
                var getValue = Expression.Call(method, parame);
                arguments.Add(getValue);
            }
            var body = Expression.New(constructor, arguments);
            var ret = Expression.Lambda<Func<DataContainer, T>>(body, parame).Compile();
            return ret;
        }
        static Func<DataContainer, T> CreateObjectGenerator2<T>(IEnumerable<Attribute.FieldMapping> mapping)
        {
            var objectType = typeof(T);
            var fields = TypeCache.GetProperties(objectType, true);
            var parame = Expression.Parameter(typeof(DataContainer), "par");
            var memberBindings = new List<MemberBinding>();
            //按顺序生成Binding
            foreach (var mp in mapping)
            {
                if (!fields.ContainsKey(mp.MappingName))
                {
                    continue;
                }
                var m = fields[mp.MappingName].GetPropertyInfo();
                var method = DataExtensions.GetMethod(m.PropertyType);
                Expression getValue = Expression.Call(method, parame);
                if (m.PropertyType.IsEnum)
                {
                    getValue = Expression.Convert(getValue, m.PropertyType);
                }
                var bind = (MemberBinding)Expression.Bind(m, getValue);
                memberBindings.Add(bind);
            }
            Expression expr = Expression.MemberInit(Expression.New(objectType), memberBindings);
            var ret = Expression.Lambda<Func<DataContainer, T>>(expr, parame);
            return ret.Compile();
        }

    }
}
