using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery.Mapping
{
    internal class QueryInfo<T>
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
                var a = DelegateCache.TryGetValue(typeof(T), out dg);
                if (a)
                {
                    ObjCreater = (Func<DataContainer, T>)dg;
                    return;
                }
                ObjCreater = CreateObjectGenerator<T>(constructor);
                DelegateCache.TryAdd(typeof(T), ObjCreater);
            }
            else
            {
                Reflection = ReflectionHelper.GetInfo<T>();
            }
        }
        public bool AnonymousClass;
        public IEnumerable<Attribute.FieldMapping> Mapping;
        public Func<DataContainer, T> ObjCreater;
        public ReflectionInfo<T> Reflection;

        Func<DataContainer, TObject> CreateObjectGenerator<TObject>(ConstructorInfo constructor)
        {
            Func<DataContainer, TObject> ret = null;
            var parame = Expression.Parameter(typeof(DataContainer), "par");
            ParameterInfo[] parameters = constructor.GetParameters();
            List<Expression> arguments = new List<Expression>(parameters.Length);
            foreach (ParameterInfo parameter in parameters)
            {
                var method = DataExtensions.GetMethod(parameter.ParameterType);
                var getValue = Expression.Call(method, parame);
                arguments.Add(getValue);
            }
            var body = Expression.New(constructor, arguments);
            ret = Expression.Lambda<Func<DataContainer, TObject>>(body, parame).Compile();
            return ret;
        }
    }
}
