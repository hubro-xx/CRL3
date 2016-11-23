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
        public QueryInfo(bool anonymousClass, List<Attribute.FieldMapping> mapping = null, ConstructorInfo constructor = null)
        {
            //AnonymousClass = anonymousClass;
            mapping = mapping ?? new List<Attribute.FieldMapping>();
            Mapping = mapping;
            if (anonymousClass)
            {
                ObjCreater = CreateObjectGenerator<T>(constructor);
            }
            else
            {
                ObjCreater = CreateObjectGenerator<T>(typeof(T).GetConstructor(Type.EmptyTypes));
                Reflection = ReflectionHelper.GetInfo<T>();
            }
        }
        //public bool AnonymousClass;
        public List<Attribute.FieldMapping> Mapping;
        public Func<DataContainer, T> ObjCreater;
        public ReflectionInfo<T> Reflection;

        Func<DataContainer, TObject> CreateObjectGenerator<TObject>(ConstructorInfo constructor)
        {
            Func<DataContainer, TObject> ret = null;
            var parame = Expression.Parameter(typeof(DataContainer), "parame");
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
