using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery.Mapping
{
    internal class QueryInfo<TSource>
    {
        static System.Collections.Concurrent.ConcurrentDictionary<string, Delegate> DelegateCache = new System.Collections.Concurrent.ConcurrentDictionary<string, Delegate>();
        public QueryInfo(bool anonymousClass, IEnumerable<Attribute.FieldMapping> mapping = null, ConstructorInfo constructor = null)
        {
            mapping = mapping ?? new List<Attribute.FieldMapping>();
            Mapping = mapping;
            AnonymousClass = anonymousClass;
            if (!Base.UseEmitCreater)
            {
                #region 按委托
                if (anonymousClass)
                {
                    var key = typeof(TSource).ToString();
                    Delegate dg;
                    //缓存处理
                    var a = DelegateCache.TryGetValue(key, out dg);
                    if (a)
                    {
                        ObjCreater = (Func<DataContainer, TSource>)dg;
                        return;
                    }
                    ObjCreater = CreateObjectGenerator<TSource>(constructor);
                    DelegateCache.TryAdd(key, ObjCreater);
                }
                else
                {
                    Reflection = ReflectionHelper.GetInfo<TSource>();
                }
                #endregion
            }
            else
            {
                #region 按EMIT创建
                var key = typeof(TSource).ToString() + string.Join("-", mapping.Select(b => b.MappingName));
                Delegate dg;
                //缓存处理
                var a = DelegateCache.TryGetValue(key, out dg);
                if (a)
                {
                    ObjCreater = (Func<DataContainer, TSource>)dg;
                    return;
                }
                else
                {
                    if (anonymousClass)
                    {
                        ObjCreater = CreateObjectGenerator<TSource>(constructor);
                    }
                    else
                    {
                        ObjCreater = CreateObjectGeneratorEmit<TSource>(mapping);
                    }
                    DelegateCache.TryAdd(key, ObjCreater);
                }
                #endregion
            }
        }
        public bool AnonymousClass;
        public IEnumerable<Attribute.FieldMapping> Mapping;
        public Func<DataContainer, TSource> ObjCreater;
        public ReflectionInfo<TSource> Reflection;

        /// <summary>
        /// 使用lambda匿名对象创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructor"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 按Lambda创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public static Func<DataContainer, T> CreateObjectGeneratorLambda<T>(IEnumerable<Attribute.FieldMapping> mapping)
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

        /// <summary>
        /// 按EMIT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public static Func<DataContainer, T> CreateObjectGeneratorEmit<T>(IEnumerable<Attribute.FieldMapping> mapping)
        {
            var type = typeof(T);
            var fields = TypeCache.GetProperties(type, true);
            DynamicMethod method = new DynamicMethod("DynamicCreateEntity", type,
                             new Type[] { typeof(CRL.LambdaQuery.Mapping.DataContainer) }, type, true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(type);
            generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);
            foreach (var mp in mapping)
            {
                if (!fields.ContainsKey(mp.MappingName))
                {
                    continue;
                }
                var pro = fields[mp.MappingName].GetPropertyInfo();
                var endIfLabel = generator.DefineLabel();
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ldarg_0);
                //generator.Emit(OpCodes.Ldc_I4, i);
                var method2 = CRL.LambdaQuery.Mapping.DataContainer.GetMethod(pro.PropertyType);
                generator.Emit(OpCodes.Call, method2);
                generator.Emit(OpCodes.Callvirt, pro.GetSetMethod());
                generator.MarkLabel(endIfLabel);
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            var handler = (Func<DataContainer, T>)method.CreateDelegate(typeof(Func<DataContainer, T>));
            return handler;
        }
    }
}
