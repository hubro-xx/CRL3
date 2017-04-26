/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CoreHelper;
using System.Reflection;
using CRL;
using System.Collections.Concurrent;

namespace CRL.LambdaQuery
{
    internal class ExpressionVisitor
    {
        DbContext dbContext;
        DBAdapter.DBAdapterBase __DBAdapter;
        /// <summary>
        /// 字段前辍 t1.
        /// </summary>
        Dictionary<Type, string> Prefixs
        {
            get
            {
                return lambdaQueryBase.__Prefixs;
            }
        }
        string __PrefixsAllKey
        {
            get
            {
                return lambdaQueryBase.__PrefixsAllKey;
            }
        }
        LambdaQueryBase lambdaQueryBase;
        public ExpressionVisitor(LambdaQueryBase _lambdaQueryBase)
        {
            lambdaQueryBase = _lambdaQueryBase;
            __DBAdapter = lambdaQueryBase.__DBAdapter;
            dbContext = __DBAdapter.dbContext;
        }

        string FormatFieldPrefix(Type type, string fieldName)
        {
            return Prefixs[type] + __DBAdapter.KeyWordFormat(fieldName);
        }

        /// <summary>
        /// 处理后的查询参数
        /// </summary>
        internal Dictionary<string, object> QueryParames = new Dictionary<string, object>();
        int parIndex
        {
            get
            {
                return dbContext.parIndex;
            }
            set
            {
                dbContext.parIndex = value;
            }
        }

        CRLExpression.CRLExpression DealParame(CRLExpression.CRLExpression par1, string typeStr)
        {
            var par = par1.Data + "";
            //typeStr2 = typeStr;
            //todo 非关系型数据库不参数化
            if (dbContext.DataBaseArchitecture == DataBaseArchitecture.NotRelation)
            {
                return par1;
            }
            if (parIndex > 1000)
            {
                throw new CRLException("参数计数超过了1000,请确认数据访问对象没有被静态化");
            }
            switch (par1.Type)
            {
                case CRLExpression.CRLExpressionType.Value:
                    #region value
                    if (par1.Data == null)
                    {
                        par = __DBAdapter.IsNotFormat(typeStr != "=") + "null";
                    }
                    else
                    {
                        QueryParames.Add("par" + parIndex, par);
                        par = "@par" + parIndex;
                        parIndex += 1;
                    }
                    #endregion
                    break;
                case CRLExpression.CRLExpressionType.MethodCall:
                    #region method
                    var method = par1.Data as CRLExpression.MethodCallObj;
                    var dic = MethodAnalyze.GetMethos(__DBAdapter);
                    if (!dic.ContainsKey(method.MethodName))
                    {
                        throw new CRLException("LambdaQuery不支持扩展方法" + method.MemberQueryName + "." + method.MethodName);
                    }
                    int newParIndex = parIndex;
                    par = dic[method.MethodName](method, ref newParIndex, AddParame);
                    parIndex = newParIndex;
                    #endregion
                    break;
            }
            par1.DataParamed = par;
            return par1;
        }
        internal static Dictionary<string, CRLExpression.CRLExpression> BinaryExpressionCache = new Dictionary<string, CRLExpression.CRLExpression>();

        static ExpressionType[] binaryTypes = new ExpressionType[] { ExpressionType.Equal, ExpressionType.GreaterThan, ExpressionType.GreaterThanOrEqual, ExpressionType.LessThan, ExpressionType.LessThanOrEqual, ExpressionType.NotEqual };
        public string DealCRLExpression(Expression exp, CRLExpression.CRLExpression b, string typeStr, out bool isNullValue, bool first = false)
        {
            isNullValue = false;
            switch (b.Type)
            {
                case CRLExpression.CRLExpressionType.Name:
                    return FormatFieldPrefix(b.MemberType, b.Data.ToString());
                case CRLExpression.CRLExpressionType.Binary:
                    return b.Data.ToString();
                default:
                    var valExp = (b.IsConstantValue || first) ? b : RouteExpressionHandler(exp);
                    isNullValue = valExp.Data == null;
                    var par2 = DealParame(valExp, typeStr);
                    return par2.DataParamed;
            }
        }
        CRLExpression.CRLExpression BinaryExpressionHandler(Expression left, Expression right, ExpressionType expType)
        {
            var isBinary = binaryTypes.Contains(expType);
            string key = "";
            string typeStr = ExpressionTypeCast(expType);
            string __typeStr2 = typeStr;
            string outLeft, outRight;
            bool isNullValue = false;
            //isBinary = false;
            if (isBinary)
            {
                #region 二元运算缓存
                CRLExpression.CRLExpression cacheItem;
                key = string.Format("{0}{1}{2}{3}", __PrefixsAllKey, left, expType, right);
                var a = BinaryExpressionCache.TryGetValue(key, out cacheItem);
                if (a)
                {
                    outLeft = DealCRLExpression(left, cacheItem.Left, typeStr, out isNullValue);
                    outRight = DealCRLExpression(right, cacheItem.Right, typeStr, out isNullValue);
                    if (isNullValue)//left为null则语法错误
                    {
                        __typeStr2 = "";
                    }
                    cacheItem.SqlOut = string.Format("({0}{1}{2})", outLeft, __typeStr2, outRight);
                    cacheItem.Data = cacheItem.SqlOut;
                    return cacheItem;
                }
                #endregion
            }
            StringBuilder sb = new StringBuilder();
            var leftPar = RouteExpressionHandler(left);
            var rightPar = RouteExpressionHandler(right);
            #region 修正bool值一元运算
            //b => b.IsTop && b.Id < 10
            if (expType == ExpressionType.AndAlso || expType == ExpressionType.OrElse)
            {
                if (leftPar.Type == CRLExpression.CRLExpressionType.Name)
                {
                    left = left.Equal(Expression.Constant(true));
                    leftPar = RouteExpressionHandler(left);
                }
                else if (rightPar.Type == CRLExpression.CRLExpressionType.Name)
                {
                    right = right.Equal(Expression.Constant(true));
                    rightPar = RouteExpressionHandler(right);
                }
            }
            #endregion
            outLeft = DealCRLExpression(left, leftPar, typeStr, out isNullValue, true);
            outRight = DealCRLExpression(right, rightPar, typeStr, out isNullValue, true);
            if (isNullValue)//left为null则语法错误
            {
                __typeStr2 = "";
            }
            sb.AppendFormat("({0}{1}{2})", outLeft, __typeStr2, outRight);
            var e = new CRLExpression.CRLExpression() { ExpType = expType.ToString(), Left = leftPar, Right = rightPar, Type = isBinary ? CRLExpression.CRLExpressionType.Binary : CRLExpression.CRLExpressionType.Tree };
            e.SqlOut = sb.ToString();
            e.Data = e.SqlOut;
            if (isBinary)
            {
                BinaryExpressionCache[key] = e;
            }
            return e;
        }
        internal static Dictionary<string, CRLExpression.CRLExpression> MemberExpressionCache = new Dictionary<string, CRLExpression.CRLExpression>();
        internal static Dictionary<string, MethodCallExpressionCacheItem> MethodCallExpressionCache = new Dictionary<string, MethodCallExpressionCacheItem>();
        internal struct MethodCallExpressionCacheItem
        {
            public CRLExpression.CRLExpression CRLExpression;
            public int argsIndex;
            public bool isConstantMethod;
            public bool isStatic;
        }
        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="nodeType"></param>
        /// <param name="firstLevel">是否首次调用,来用修正bool一元运算</param>
        /// <returns></returns>
        public CRLExpression.CRLExpression RouteExpressionHandler(Expression exp, ExpressionType? nodeType = null, bool firstLevel = false)
        {
            if (exp is BinaryExpression)
            {
                BinaryExpression be = (BinaryExpression)exp;
                return BinaryExpressionHandler(be.Left, be.Right, be.NodeType);
            }
            if (exp is MemberExpression)
            {
                return MemberExpressionHandler(exp, nodeType, firstLevel);
            }
            else if (exp is ConstantExpression)
            {
                return ConstantExpressionHandler(exp, nodeType, firstLevel);
            }
            else if (exp is MethodCallExpression)
            {
                return MethodCallExpressionHandler(exp, nodeType, firstLevel);
            }
            else if (exp is UnaryExpression)
            {
                return UnaryExpressionHandler(exp, nodeType, firstLevel);
            }
            else if (exp is NewArrayExpression)
            {
                return NewArrayExpressionHandler(exp, nodeType, firstLevel);
            }
            else
            {
                throw new CRLException("不支持此语法解析:" + exp);
            }
        }
        public void AddParame(string name, object value)
        {
            QueryParames.Add(name, value);
            //parIndex += 1;
        }
        static ConcurrentDictionary<ExpressionType, string> expressionTypeCache = new ConcurrentDictionary<ExpressionType, string>();
        public static string ExpressionTypeCast(ExpressionType expType)
        {
            if (expressionTypeCache.Count == 0)
            {
                expressionTypeCache.TryAdd(ExpressionType.And, "&");
                expressionTypeCache.TryAdd(ExpressionType.AndAlso, " AND ");
                expressionTypeCache.TryAdd(ExpressionType.Equal, "=");
                expressionTypeCache.TryAdd(ExpressionType.GreaterThan, ">");
                expressionTypeCache.TryAdd(ExpressionType.GreaterThanOrEqual, ">=");
                expressionTypeCache.TryAdd(ExpressionType.LessThan, "<");
                expressionTypeCache.TryAdd(ExpressionType.LessThanOrEqual, "<=");
                expressionTypeCache.TryAdd(ExpressionType.NotEqual, "<>");
                expressionTypeCache.TryAdd(ExpressionType.Or, "|");
                expressionTypeCache.TryAdd(ExpressionType.OrElse, " OR ");
                expressionTypeCache.TryAdd(ExpressionType.Add, "+");
                expressionTypeCache.TryAdd(ExpressionType.AddChecked, "+");
                expressionTypeCache.TryAdd(ExpressionType.Subtract, "-");
                expressionTypeCache.TryAdd(ExpressionType.SubtractChecked, "-");
                expressionTypeCache.TryAdd(ExpressionType.Multiply, "*");
                expressionTypeCache.TryAdd(ExpressionType.MultiplyChecked, "*");
                expressionTypeCache.TryAdd(ExpressionType.Divide, "/");
                expressionTypeCache.TryAdd(ExpressionType.Not, "!=");
            }
            string type;
            var a = expressionTypeCache.TryGetValue(expType, out type);
            if (a)
            {
                return type;
            }
            throw new InvalidCastException("不支持的运算符" + expType);

        }
        object GetParameExpressionValue(Expression expression, out bool isConstant)
        {
            isConstant = false;
            //只能处理常量
            if (expression is ConstantExpression)
            {
                isConstant = true;
                ConstantExpression cExp = (ConstantExpression)expression;
                return cExp.Value;
            }
            else if (expression is MemberExpression)//按属性访问
            {
                var m = expression as MemberExpression;
                if (m.Expression != null)
                {
                    if (m.Expression.NodeType == ExpressionType.Parameter)
                    {
                        string name = m.Member.Name;
                        var filed2 = TypeCache.GetProperties(m.Expression.Type, true)[name];
                        return new ExpressionValueObj { Value = FormatFieldPrefix(m.Expression.Type, filed2.MapingName), IsMember = true };
                    }
                    else
                    {
                        return ConstantValueVisitor.GetMemberExpressionValue(m, out isConstant);
                    }
                }
                return ConstantValueVisitor.GetMemberExpressionValue(m, out isConstant);
            }
            //按编译
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }
        #region 按类型解析
        CRLExpression.CRLExpression MemberExpressionHandler(Expression exp, ExpressionType? nodeType = null, bool firstLevel = false)
        {
            #region MemberExpression
            //区分 属性表达带替换符{0} 变量值不带
            MemberExpression mExp = (MemberExpression)exp;
            if (mExp.Expression != null && mExp.Expression.NodeType == ExpressionType.Parameter) //like b.Name==b.Name1 或b.Name
            {
                #region MemberParameter
                var fieldName = mExp.Member.Name;
                var type = mExp.Expression.Type;
                if (mExp.Member.ReflectedType.Name.StartsWith("<>f__AnonymousType"))//按匿名类
                {
                    //var queryField = FormatFieldPrefix(type, fieldName);
                    var exp2 = new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Name, Data = fieldName, MemberType = type };
                    //MemberExpressionCache[key] = exp2;
                    return exp2;
                }

                CRL.Attribute.FieldAttribute field;
                var a = TypeCache.GetProperties(type, true).TryGetValue(fieldName, out field);
                if (!a)
                {
                    throw new CRLException("类型 " + type.Name + "." + fieldName + " 不是数据库字段,请检查查询条件");
                }
                //if (!string.IsNullOrEmpty(field.VirtualField))//按虚拟字段
                //{
                //    //return filed.VirtualField;
                //    var queryField = field.VirtualField.Replace("{" + type.FullName + "}", Prefixs[type]);//替换前辍
                //    var exp2 = new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Name, Data = queryField, MemberType = type };
                //    //MemberExpressionCache[key] = exp2;
                //    return exp2;
                //}
                //var fieldStr = FormatFieldPrefix(type, field.MapingName);//格式化为别名
                var fieldStr = field.MapingName;
                //return field;
                if (firstLevel)
                {
                    var exp2 = exp.Equal(Expression.Constant(true));
                    return RouteExpressionHandler(exp2);
                }
                var exp3 = new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Name, Data = fieldStr, MemberType = type };
                //MemberExpressionCache[key] = exp3;
                return exp3;
                #endregion
            }
            else
            {
                string key = exp.ToString() + firstLevel;
                CRLExpression.CRLExpression val;
                var a1 = MemberExpressionCache.TryGetValue(key, out val);
                if (a1)
                {
                    return val;
                }
                #region 按值
                bool isConstant;
                var obj = GetParameExpressionValue(mExp, out isConstant);
                if (obj is Enum)
                {
                    obj = (int)obj;
                }
                else if (obj is Boolean)//sql2000需要转换
                {
                    obj = Convert.ToInt32(obj);
                }
                var exp4 = new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Value, Data = obj, IsConstantValue = isConstant };
                if (isConstant)
                {
                    MemberExpressionCache[key] = exp4;
                }
                return exp4;
                #endregion
            }
            #endregion
        }
        CRLExpression.CRLExpression NewArrayExpressionHandler(Expression exp, ExpressionType? nodeType = null, bool firstLevel = false)
        {
            #region 数组
            NewArrayExpression naExp = (NewArrayExpression)exp;
            StringBuilder sb = new StringBuilder();
            foreach (Expression expression in naExp.Expressions)
            {
                sb.AppendFormat(",{0}", RouteExpressionHandler(expression));
            }
            var str = sb.Length == 0 ? "" : sb.Remove(0, 1).ToString();
            return new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Value, Data = str };
            #endregion
        }
        CRLExpression.CRLExpression MethodCallExpressionHandler(Expression exp, ExpressionType? nodeType = null, bool firstLevel = false)
        {
            #region methodCall
            MethodCallExpression mcExp = (MethodCallExpression)exp;
            var arguments = new List<object>();
            var allArguments = mcExp.Arguments;
            int argsIndex = 0;
            Expression firstArgs;
            if (mcExp.Method.IsStatic)//区分静态方法还是实例方法
            {
                firstArgs = allArguments[0];//like b.Name.IsNull("22")
                argsIndex = 1;
            }
            else
            {
                firstArgs = mcExp.Object;//like b.Id.ToString()
            }

            MethodCallExpressionCacheItem methodCache;
            #region 缓存处理
            string key = __PrefixsAllKey + mcExp + nodeType;
            var exists = MethodCallExpressionCache.TryGetValue(key, out methodCache);
            if (exists)
            {
                if (!methodCache.isConstantMethod)
                {
                    var methodCall = methodCache.CRLExpression.Data as CRLExpression.MethodCallObj;
                    if (methodCall.Args.Count > 0)
                    {
                        for (int i = argsIndex; i < allArguments.Count; i++)
                        {
                            bool isConstant1;
                            var obj = GetParameExpressionValue(allArguments[i], out isConstant1);
                            arguments.Add(obj);
                        }
                        methodCall.Args = arguments;
                    }
                }
                methodCache.CRLExpression.SqlOut = "";
                return methodCache.CRLExpression;
            }
            #endregion
            #region MethodCallExpression
            bool isConstantMethod = false;

            MemberExpression memberExpression;

            string methodField = "";
            string memberName = "";
            string methodName = mcExp.Method.Name;

            if (firstArgs is ParameterExpression)
            {
                var exp2 = mcExp.Arguments[1] as UnaryExpression;
                var type = exp2.Operand.GetType();
                var p = type.GetProperty("Body");
                var exp3 = p.GetValue(exp2.Operand, null) as Expression;
                methodField = RouteExpressionHandler(exp3).SqlOut;
                memberName = "";
            }
            else if (firstArgs is UnaryExpression)//like a.Code.Count()
            {
                memberExpression = (firstArgs as UnaryExpression).Operand as MemberExpression;
                memberName = memberExpression.Member.Name;
                methodField = FormatFieldPrefix(memberExpression.Expression.Type, memberExpression.Member.Name);
            }
            else if (firstArgs is MemberExpression)
            {
                //like a.Code
                memberExpression = firstArgs as MemberExpression;
                memberName = memberExpression.Member.Name;
                var type = memberExpression.Expression.Type;
                if (type.IsSubclassOf(typeof(IModel)))
                {
                    memberName = TypeCache.GetProperties(type, true)[memberExpression.Member.Name].MapingName;
                }
                if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
                {
                    methodField = FormatFieldPrefix(memberExpression.Expression.Type, memberName);
                    var allConstant = true;
                    for (int i = argsIndex; i < allArguments.Count; i++)
                    {
                        bool isConstant2;
                        var obj = GetParameExpressionValue(allArguments[i], out isConstant2);
                        if (!isConstant2)
                        {
                            allConstant = false;
                        }
                        arguments.Add(obj);
                    }
                    if (allConstant)
                    {
                        isConstantMethod = true;
                    }
                }
                else
                {
                    //like Convert.ToDateTime(times)
                    var obj = ConstantValueVisitor.GetParameExpressionValue(firstArgs);
                    arguments.Add(obj);
                    for (int i = argsIndex; i < allArguments.Count; i++)
                    {
                        bool isConstant2;
                        var obj2 = GetParameExpressionValue(allArguments[i], out isConstant2);
                        arguments.Add(obj2);
                    }
                }
            }
            else if (firstArgs is ConstantExpression)//按常量
            {
                //like DateTime.Parse("2016-02-11 12:56"),
                isConstantMethod = true;
                var obj = ConstantValueVisitor.GetParameExpressionValue(firstArgs);
                arguments.Add(obj);
            }
            //else
            //{
            //    throw new CRLException("不支持此语法解析:" + args);
            //}

            if (nodeType == null)
            {
                nodeType = ExpressionType.Equal;
            }
            if (string.IsNullOrEmpty(methodField))
            {
                //当是常量转换方法
                
                var method = mcExp.Method;
                object obj;
                if (mcExp.Method.IsStatic)//like DateTime.Parse("2016-02-11")
                {
                    obj = method.Invoke(null, arguments.ToArray());
                }
                else//like time.AddDays(1)
                {
                    var args1 = arguments.First();
                    arguments.RemoveAt(0);
                    obj = method.Invoke(args1, arguments.ToArray());
                }
                var exp2 = new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Value, Data = obj };
                var cache = new MethodCallExpressionCacheItem() { CRLExpression = exp2, argsIndex = argsIndex, isConstantMethod = isConstantMethod, isStatic = mcExp.Method.IsStatic };
                //MethodCallExpressionCache[key] = cache;
                return exp2;
            }
            var methodInfo = new CRLExpression.MethodCallObj() { Args = arguments, ExpressionType = nodeType.Value, MemberName = memberName, MethodName = methodName, MemberQueryName = methodField };
            methodInfo.ReturnType = mcExp.Type;
            #endregion
            var exp4 = new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.MethodCall, Data = methodInfo };
            var cache2 = new MethodCallExpressionCacheItem() { CRLExpression = exp4, argsIndex = argsIndex, isConstantMethod = isConstantMethod };
            MethodCallExpressionCache[key] = cache2;
            return exp4;
            #endregion
        }
        CRLExpression.CRLExpression ConstantExpressionHandler(Expression exp, ExpressionType? nodeType = null, bool firstLevel = false)
        {
            #region 常量
            ConstantExpression cExp = (ConstantExpression)exp;
            object returnValue;
            if (cExp.Value == null)
            {
                returnValue = null;
            }
            else
            {
                if (cExp.Value is Boolean)
                {
                    returnValue = Convert.ToInt32(cExp.Value).ToString();
                }
                else if (cExp.Value is Enum)
                {
                    returnValue = Convert.ToInt32(cExp.Value).ToString();
                }
                else
                {
                    returnValue = cExp.Value;
                }
            }
            return new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Value, Data = returnValue, IsConstantValue = true };
            #endregion
        }
        CRLExpression.CRLExpression UnaryExpressionHandler(Expression exp, ExpressionType? nodeType = null, bool firstLevel = false)
        {
            #region UnaryExpression
            UnaryExpression ue = ((UnaryExpression)exp);
            if (ue.Operand is MethodCallExpression)
            {
                //方法直接下一步解析
                return RouteExpressionHandler(ue.Operand, ue.NodeType);
            }
            else if (ue.Operand is MemberExpression)
            {
                MemberExpression mExp = (MemberExpression)ue.Operand;
                if (mExp.Expression.NodeType != ExpressionType.Parameter)
                {
                    return RouteExpressionHandler(ue.Operand);
                }
                var parameter = Expression.Parameter(mExp.Expression.Type, "b");
                if (ue.NodeType == ExpressionType.Not)
                {
                    var ex2 = parameter.Property(mExp.Member.Name).Equal(1);
                    return RouteExpressionHandler(ex2);
                }
                else if (ue.NodeType == ExpressionType.Convert)
                {
                    //like Convert(b.Id);
                    var ex2 = parameter.Property(mExp.Member.Name);
                    return RouteExpressionHandler(ex2);

                }
            }
            else if (ue.Operand is ConstantExpression)
            {
                return RouteExpressionHandler(ue.Operand);
            }
            throw new CRLException("未处理的一元运算" + ue.NodeType);
            #endregion
        }
        #endregion
    }
    internal class ExpressionValueObj
    {
        public object Value;
        public bool IsMember;
        public override string ToString()
        {
            return Value + "";
        }
    }
}
