/**
* CRL 快速开发框架 V4.5
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
        DbContext dbContext
        {
            get
            {
                return lambdaQueryBase.__DbContext;
            }
        }
        DBAdapter.DBAdapterBase __DBAdapter
        {
            get
            {
                return lambdaQueryBase.__DBAdapter;
            }
        }
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
        //string __PrefixsAllKey
        //{
        //    get
        //    {
        //        return lambdaQueryBase.__PrefixsAllKey;
        //    }
        //}
        LambdaQueryBase lambdaQueryBase;
        public ExpressionVisitor(LambdaQueryBase _lambdaQueryBase)
        {
            lambdaQueryBase = _lambdaQueryBase;
            //__DBAdapter = lambdaQueryBase.__DBAdapter;
            //dbContext = __DBAdapter.dbContext;
        }

        string FormatFieldPrefix(Type type, string fieldName)
        {
            return Prefixs[type] + fieldName;
        }

        /// <summary>
        /// 处理后的查询参数
        /// </summary>
        internal List<Tuple<string, object>> QueryParames = new List<Tuple<string, object>>();
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
        static string[] parameDic = null;
        CRLExpression.CRLExpression DealParame(CRLExpression.CRLExpression par1, string typeStr)
        {
            var par = par1.Data + "";
            //typeStr2 = typeStr;
            //todo 非关系型数据库不参数化
            if (dbContext.DataBaseArchitecture == DataBaseArchitecture.NotRelation)
            {
                return par1;
            }
            if (parameDic == null)
            {
                parameDic = new string[5000];
                for (int i = 0; i < 5000; i++)
                {
                    parameDic[i] = __DBAdapter.GetParamName("p", i);
                }
            }
            if (parIndex >= 5000)
            {
                //MSSQL 参数最多2800
                throw new CRLException("参数计数超过了5000,请确认数据访问对象没有被静态化" + parIndex);
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
                        if(SettingConfig.FieldParameName)
                        {
                            par1.DataParamed = par;
                            return par1;//参数名按字段名
                        }
                        var _par = parameDic[parIndex];
                        AddParame(_par, par);
                        par = _par;
                        parIndex += 1;
                    }
                    #endregion
                    break;
                case CRLExpression.CRLExpressionType.MethodCall:
                    #region method
                    var method = par1.Data as CRLExpression.MethodCallObj;

                    #region in优化 MSSQL内部已自动优化
                    var nodeType = method.ExpressionType;
                    /**
                    if (dbContext.DataBaseArchitecture == DataBaseArchitecture.Relation && method.MethodName == "In" && lambdaQueryBase.__AutoInJoin > 5 && nodeType == ExpressionType.Equal)
                    {
                        var inArgs = method.Args[0] as System.Collections.IEnumerable;
                        int _i = 0;
                        Type innerType = null;
                        var pars = new List<InJoin>();
                        var batchNo = System.Guid.NewGuid().ToString().Replace("-", "");
                        foreach (var a in inArgs)
                        {
                            _i += 1;
                            if (innerType == null)
                            {
                                innerType = a.GetType();
                            }
                            var obj2 = new InJoin() { BatchNo = batchNo, V_String = "" };
                            obj2.SetValue(innerType, a);
                            pars.Add(obj2);
                        }
                        if (_i > lambdaQueryBase.__AutoInJoin)
                        {
                            QueryParames.Add("__batchNO", batchNo);
                            var typeJoin = typeof(InJoin);
                            var inJoinName = lambdaQueryBase.GetPrefix(typeJoin);
                            var joinFormat = string.Format("{1}BatchNo=@__batchNO and {0}={1}V_{2}", method.MemberQueryName, inJoinName, innerType.Name);
                            lambdaQueryBase.AddInnerRelation(new TypeQuery(typeJoin), JoinType.Inner, joinFormat);
                            par1.DataParamed = "";
                            var dbEx = DBExtendFactory.CreateDBExtend(dbContext);
                            dbEx.CheckTableCreated(typeJoin);
                            __DBAdapter.BatchInsert(pars);
                            lambdaQueryBase.__RemoveInJionBatchNo = batchNo;
                            return par1;
                        }
                    }
                    */
                    #endregion

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
        //internal static Dictionary<string, CRLExpression.CRLExpression> BinaryExpressionCache = new Dictionary<string, CRLExpression.CRLExpression>();

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
        List<string> existsTempParameName;
        CRLExpression.CRLExpression BinaryExpressionHandler(Expression left, Expression right, ExpressionType expType)
        {
            var isBinary = binaryTypes.Contains(expType);
            //string key = "";
            string typeStr = ExpressionTypeCast(expType);
            string __typeStr2 = typeStr;
            string outLeft, outRight;
            bool isNullValue = false;
            //isBinary = false;
            if (isBinary)
            {
                #region 二元运算缓存 并没有效率提升
                //if (SettingConfig.UseLambdaCache&&false)
                //{
                //    CRLExpression.CRLExpression cacheItem;
                //    key = string.Format("{0}{1}{2}{3}", __PrefixsAllKey, left, expType, right);
                //    var a = BinaryExpressionCache.TryGetValue(key, out cacheItem);
                //    if (a)
                //    {
                //        outLeft = DealCRLExpression(left, cacheItem.Left, typeStr, out isNullValue);
                //        outRight = DealCRLExpression(right, cacheItem.Right, typeStr, out isNullValue);
                //        if (isNullValue)//left为null则语法错误
                //        {
                //            __typeStr2 = "";
                //        }
                //        cacheItem.SqlOut = string.Format("({0}{1}{2})", outLeft, __typeStr2, outRight);
                //        cacheItem.Data = cacheItem.SqlOut;
                //        return cacheItem;
                //    }
                //}
                #endregion
            }
            var sb = "";
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
            #region 固定名称的参数
            if(isBinary&& SettingConfig.FieldParameName)
            {
                if ((int)(leftPar.Type | rightPar.Type) == 12)
                {
                    CRLExpression.CRLExpression tempName;
                    CRLExpression.CRLExpression tempValue;
                    if (leftPar.Type == CRLExpression.CRLExpressionType.Name)
                    {
                        tempName = leftPar;
                        tempValue = rightPar;
                    }
                    else
                    {
                        tempName = rightPar;
                        tempValue = leftPar;
                        outLeft = outRight;
                    }
                    existsTempParameName = existsTempParameName ?? new List<string>();
                    var pre = Prefixs[tempName.MemberType];
                    pre = pre.Replace(".", $"_");
                    var pName = __DBAdapter.GetParamName(pre, tempName.Data_);
                    if (existsTempParameName.Contains(pName))
                    {
                        pName = pName.Replace("_", "_" + existsTempParameName.Count + "_");
                    }
                    AddParame(pName, tempValue.Data);
                    existsTempParameName.Add(pName);
                    outRight = pName;
                }
            }
            #endregion
            if (isNullValue)//left为null则语法错误
            {
                __typeStr2 = "";
            }
            sb = string.Format("({0}{1}{2})", outLeft, __typeStr2, outRight);
            var e = new CRLExpression.CRLExpression() { ExpType = expType, Left = leftPar, Right = rightPar, Type = isBinary ? CRLExpression.CRLExpressionType.Binary : CRLExpression.CRLExpressionType.Tree };
            e.SqlOut = sb;
            e.Data = e.SqlOut;
            //if (isBinary && SettingConfig.UseLambdaCache && false)
            //{
            //    BinaryExpressionCache[key] = e;
            //}
            return e;
        }
        //internal static Dictionary<string, CRLExpression.CRLExpression> MemberExpressionCache = new Dictionary<string, CRLExpression.CRLExpression>();
        //internal static Dictionary<string, MethodCallExpressionCacheItem> MethodCallExpressionCache = new Dictionary<string, MethodCallExpressionCacheItem>();
        //internal struct MethodCallExpressionCacheItem
        //{
        //    public CRLExpression.CRLExpression CRLExpression;
        //    public int argsIndex;
        //    public bool isConstantMethod;
        //    public bool isStatic;
        //}
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
            QueryParames.Add(new Tuple<string, object>(name,value));
            //parIndex += 1;
        }
        static ConcurrentDictionary<ExpressionType, string> expressionTypeCache = null;
        static object lockObj = new object();
        public static string ExpressionTypeCast(ExpressionType expType)
        {
            if (expressionTypeCache == null)
            {
                lock (lockObj)
                {
                    expressionTypeCache = new ConcurrentDictionary<ExpressionType, string>();
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
                        return new ExpressionValueObj { Value = FormatFieldPrefix(m.Expression.Type, __DBAdapter.FieldNameFormat(filed2)), IsMember = true };
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
                var memberName = mExp.Member.Name;
                var type = mExp.Expression.Type;
                if (mExp.Member.ReflectedType.Name.StartsWith("<>f__AnonymousType"))//按匿名类
                {
                    var exp2 = new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Name, Data = memberName, MemberType = type };
                    return exp2;
                }
                if (firstLevel)//没有运算符的BOOL判断
                {
                    var exp2 = exp.Equal(Expression.Constant(true));
                    return RouteExpressionHandler(exp2);
                }
                CRL.Attribute.FieldAttribute field;
                var a = TypeCache.GetProperties(type, true).TryGetValue(memberName, out field);
                if (!a)
                {
                    throw new CRLException("类型 " + type.Name + "." + memberName + " 未设置Mapping,请检查查询条件");
                }
                if (field.DefaultCRLExpression != null)
                {
                    var c2 = field.DefaultCRLExpression;
                    c2.Data = __DBAdapter.FieldNameFormat(field);
                    return c2;
                }
                var fieldStr = __DBAdapter.FieldNameFormat(field);
                var exp3 = new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Name, Data = fieldStr, Data_ = field.MapingName, MemberType = type };
                field.DefaultCRLExpression = exp3;
                return exp3;
                #endregion
            }
            else
            {
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
                //if (isConstant)
                //{
                //    MemberExpressionCache[key] = exp4;
                //}
                return exp4;
                #endregion
            }
            #endregion
        }
        CRLExpression.CRLExpression NewArrayExpressionHandler(Expression exp, ExpressionType? nodeType = null, bool firstLevel = false)
        {
            #region 数组
            NewArrayExpression naExp = (NewArrayExpression)exp;
            var sb = "";
            foreach (Expression expression in naExp.Expressions)
            {
                sb += string.Format(",{0}", RouteExpressionHandler(expression));
            }
            var str = sb.Length == 0 ? "" : sb.Remove(0, 1);
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
            bool isLambdaQueryJoinExt = false;
            if (mcExp.Method.IsStatic)//区分静态方法还是实例方法
            {
                firstArgs = allArguments[0];//like b.Name.IsNull("22")
                argsIndex = 1;
                if(firstArgs.Type.Name.Contains("LambdaQueryJoin"))
                {
                    isLambdaQueryJoinExt = true;
                }
            }
            else
            {
                firstArgs = mcExp.Object;//like b.Id.ToString()
            }

            #region 缓存处理 效率并不高
            //MethodCallExpressionCacheItem methodCache;
            //string key = "";
            //if (SettingConfig.UseLambdaCache&& false)
            //{
            //    key = __PrefixsAllKey + mcExp + nodeType;
            //    var exists = MethodCallExpressionCache.TryGetValue(key, out methodCache);
            //    if (exists)
            //    {
            //        if (!methodCache.isConstantMethod)
            //        {
            //            var methodCall = methodCache.CRLExpression.Data as CRLExpression.MethodCallObj;
            //            if (methodCall.Args.Count > 0)
            //            {
            //                for (int i = argsIndex; i < allArguments.Count; i++)
            //                {
            //                    bool isConstant1;
            //                    var obj = GetParameExpressionValue(allArguments[i], out isConstant1);
            //                    arguments.Add(obj);
            //                }
            //                methodCall.Args = arguments;
            //            }
            //        }
            //        methodCache.CRLExpression.SqlOut = "";
            //        return methodCache.CRLExpression;
            //    }
            //}
            #endregion
            #region MethodCallExpression
            //bool isConstantMethod = false;

            MemberExpression memberExpression;

            string methodField = "";
            string memberName = "";
            string methodName = mcExp.Method.Name;

            if (firstArgs is ParameterExpression || isLambdaQueryJoinExt)
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
                var field = TypeCache.GetProperties(memberExpression.Expression.Type, true)[memberName];
                memberName = __DBAdapter.FieldNameFormat(field);
                methodField = FormatFieldPrefix(memberExpression.Expression.Type, memberName);
            }
            else if (firstArgs is BinaryExpression)
            {
                var be = firstArgs as BinaryExpression;
                methodField = BinaryExpressionHandler(be.Left, be.Right, be.NodeType).Data.ToString();
            }
            else if (firstArgs is MemberExpression)
            {
                //like a.Code
                memberExpression = firstArgs as MemberExpression;
                memberName = memberExpression.Member.Name;
                var type = memberExpression.Expression.Type;
                if (type.IsSubclassOf(typeof(IModel)))
                {
                    var field = TypeCache.GetProperties(type, true)[memberExpression.Member.Name];
                    memberName = __DBAdapter.FieldNameFormat(field);
                }
                if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
                {
                    methodField = FormatFieldPrefix(memberExpression.Expression.Type, memberName);
                    //var allConstant = true;
                    for (int i = argsIndex; i < allArguments.Count; i++)
                    {
                        bool isConstant2;
                        var obj = GetParameExpressionValue(allArguments[i], out isConstant2);
                        arguments.Add(obj);
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
                //isConstantMethod = true;
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
                    if (arguments.Count == 0)
                    {
                        throw new Exception("未能解析" + exp);
                    }
                    var args1 = arguments.First();
                    arguments.RemoveAt(0);
                    obj = method.Invoke(args1, arguments.ToArray());
                }
                var exp2 = new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Value, Data = obj };
                //var cache = new MethodCallExpressionCacheItem() { CRLExpression = exp2, argsIndex = argsIndex, isConstantMethod = isConstantMethod, isStatic = mcExp.Method.IsStatic };
                //MethodCallExpressionCache[key] = cache;
                return exp2;
            }
            var methodInfo = new CRLExpression.MethodCallObj() { Args = arguments, ExpressionType = nodeType.Value, MemberName = memberName, MethodName = methodName, MemberQueryName = methodField };
            methodInfo.ReturnType = mcExp.Type;
            #endregion
            var exp4 = new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.MethodCall, Data = methodInfo };
            //if (SettingConfig.UseLambdaCache && false)
            //{
            //    var cache2 = new MethodCallExpressionCacheItem() { CRLExpression = exp4, argsIndex = argsIndex, isConstantMethod = isConstantMethod };
            //    MethodCallExpressionCache[key] = cache2;
            //}
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
            else if(ue.NodeType == ExpressionType.Convert)
            {
                return RouteExpressionHandler(ue.Operand);
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
