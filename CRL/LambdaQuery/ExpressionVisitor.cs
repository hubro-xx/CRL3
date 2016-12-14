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
        internal ParameCollection QueryParames = new ParameCollection();
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
        public CRLExpression.CRLExpression DealParame(CRLExpression.CRLExpression par1, string typeStr, out string typeStr2)
        {

            var par = par1.Data + "";
            typeStr2 = typeStr;
            //todo 非关系型数据库不参数化
            if (dbContext.DataBaseArchitecture == DataBaseArchitecture.NotRelation)
            {
                return par1;
            }
            if (par1.Data is CRLExpression.MethodCallObj)
            {
                var method = par1.Data as CRLExpression.MethodCallObj;
                var _DBAdapter = DBAdapter.DBAdapterBase.GetDBAdapterBase(dbContext);
                var dic = MethodAnalyze.GetMethos(_DBAdapter);
                if (!dic.ContainsKey(method.MethodName))
                {
                    throw new CRLException("LambdaQuery不支持扩展方法" + method.MemberQueryName + "." + method.MethodName);
                }
                int newParIndex = parIndex;
                par = dic[method.MethodName](method, ref newParIndex, AddParame);
                parIndex = newParIndex;
            }

            //字段会返回替换符
            bool needPar = par1.Type == CRLExpression.CRLExpressionType.Value;//是否需要参数化处理
            if (!needPar)
            {
                par1.DataParamed = par;
                return par1;
            }
            if (needPar)
            {
                if (par.ToUpper() == "NULL")
                {
                    typeStr2 = "";
                    if (typeStr == "=")
                        par = " IS NULL ";
                    else if (typeStr == "<>")
                        par = " IS NOT NULL ";
                }
                else
                {
                    QueryParames.Add("par" + parIndex, par);
                    par = "@par" + parIndex;
                    parIndex += 1;
                }
            }
            par1.DataParamed = par;
            return par1;
        }
        static Dictionary<string, CRLExpression.CRLExpression> BinaryExpressionCache = new Dictionary<string, CRLExpression.CRLExpression>();
        bool IsMemberParameter(Expression exp)
        {
            if (exp is MemberExpression)
            {
                var mExp = exp as MemberExpression;
                if (mExp.Expression == null)
                    return false;
                return mExp.Expression.NodeType == ExpressionType.Parameter;
            }
            return false;
        }
        static ExpressionType[] binaryTypes= new ExpressionType[] { ExpressionType.Equal, ExpressionType.GreaterThan, ExpressionType.GreaterThanOrEqual, ExpressionType.LessThan, ExpressionType.LessThanOrEqual, ExpressionType.NotEqual };
        CRLExpression.CRLExpression BinaryExpressionHandler(Expression left, Expression right, ExpressionType type)
        {
            var isBinary = binaryTypes.Contains(type);
            string key = "";
            string typeStr = ExpressionTypeCast(type);
            if (isBinary)
            {
                #region 二元运算缓存
                CRLExpression.CRLExpression cacheItem;
                key = string.Join("-", Prefixs) + left + type.ToString() + right;
                var a = BinaryExpressionCache.TryGetValue(key, out cacheItem);
                if (a)
                {
                    CRLExpression.CRLExpression p2;
                    string __typeStr2;
                    if (IsMemberParameter(left))
                    {
                        p2 = RouteExpressionHandler(right);
                        var par2 = DealParame(p2, typeStr, out __typeStr2);
                        cacheItem.SqlOut = string.Format("({0}{1}{2})", cacheItem.Left.Data, __typeStr2, par2.DataParamed);
                    }
                    else
                    {
                        p2 = RouteExpressionHandler(left);
                        var par2 = DealParame(p2, typeStr, out __typeStr2);
                        cacheItem.SqlOut = string.Format("({0}{1}{2})", par2.DataParamed, __typeStr2, cacheItem.Right.Data);
                    }
                    cacheItem.Data = cacheItem.SqlOut;
                    return cacheItem;
                }
                #endregion
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            var leftPar = RouteExpressionHandler(left);
            var rightPar = RouteExpressionHandler(right);
            #region 修正bool值一元运算
            //t1.isTop=1
            if (leftPar.Type == CRLExpression.CRLExpressionType.Binary && rightPar.Type == CRLExpression.CRLExpressionType.Name)
            {
                var proType = ((MemberExpression)right).Type;
                if (proType == typeof(bool))
                {
                    rightPar.Data = rightPar.Data.ToString().Split('=')[0] + "=1";
                }
            }
            else if (rightPar.Type == CRLExpression.CRLExpressionType.Binary && leftPar.Type == CRLExpression.CRLExpressionType.Name)
            {
                var proType = ((MemberExpression)left).Type;
                if (proType == typeof(bool))
                {
                    leftPar.Data = leftPar.Data.ToString().Split('=')[0] + "=1";
                }
            }
            #endregion
            string typeStr2;
            leftPar = DealParame(leftPar, typeStr, out typeStr2);
            sb.Append(leftPar.DataParamed);
            rightPar = DealParame(rightPar, typeStr, out typeStr2);
            sb.Append(typeStr2);
            sb.Append(rightPar.DataParamed);
            sb.Append(")");
            //return sb.ToString();

            var e = new CRLExpression.CRLExpression() { ExpressionType = type.ToString(), Left = leftPar, Right = rightPar, Type = isBinary ? CRLExpression.CRLExpressionType.Binary : CRLExpression.CRLExpressionType.Tree };
            e.SqlOut = sb.ToString();
            e.Data = e.SqlOut;
            if (isBinary)
            {
                BinaryExpressionCache[key] = e;
            }
            return e;
        }
        static Dictionary<string, CRLExpression.CRLExpression> MemberExpressionCache = new Dictionary<string, CRLExpression.CRLExpression>();
        static Dictionary<string, MethodCallExpressionCacheItem> MethodCallExpressionCache = new Dictionary<string, MethodCallExpressionCacheItem>();
        class MethodCallExpressionCacheItem
        {
            public CRLExpression.CRLExpression CRLExpression;
            public int argsIndex;//为0则是实例方法
            public bool isConstantMethod;
            public bool isStatic;
        }

        /// <summary>
        /// 
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
            else if (exp is MemberExpression)
            {
                //区分 属性表达带替换符{0} 变量值不带
                #region Member
                MemberExpression mExp = (MemberExpression)exp;
                string key = string.Join("-", Prefixs) + exp + firstLevel;
                CRLExpression.CRLExpression val;
                var a1 = MemberExpressionCache.TryGetValue(key, out val);
                if (a1)
                {
                    return val;
                }

                if (mExp.Expression != null && mExp.Expression.NodeType == ExpressionType.Parameter) //like b.Name==b.Name1 或b.Name
                {
                    var fieldName = mExp.Member.Name;
                    var type = mExp.Expression.Type;
                    if (mExp.Member.ReflectedType.Name.StartsWith("<>f__AnonymousType"))//按匿名类
                    {
                        var queryField = FormatFieldPrefix(type, fieldName);
                        var exp2 = new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Name, Data = queryField };
                        MemberExpressionCache[key] = exp2;
                        return exp2;
                    }
 
                    CRL.Attribute.FieldAttribute field ;
                    var a = TypeCache.GetProperties(type, true).TryGetValue(fieldName, out field);
                    if (!a)
                    {
                        throw new CRLException("类型 " + type.Name + "." + fieldName + " 不是数据库字段,请检查查询条件");
                    }
                    if (!string.IsNullOrEmpty(field.VirtualField))//按虚拟字段
                    {
                        //return filed.VirtualField;
                        var queryField = field.VirtualField.Replace("{" + type.FullName + "}", Prefixs[type]);//替换前辍
                        var exp2= new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Name, Data = queryField };
                        MemberExpressionCache[key] = exp2;
                        return exp2;
                    }
                    var fieldStr = FormatFieldPrefix(type, field.MapingName);//格式化为别名
                    //return field;
                    if (firstLevel)
                    {
                        //修正bool值一元运算 t1.isTop=1
                        fieldStr += "=1";
                    }
                    var exp3 = new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Name, Data = fieldStr };
                    MemberExpressionCache[key] = exp3;
                    return exp3;
                }
                #endregion
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
                var exp4= new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Value, Data = obj, IsConstantValue = isConstant };
                if (isConstant)
                {
                    MemberExpressionCache[key] = exp4;
                }
                return exp4;
            }
            else if (exp is NewArrayExpression)
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
            else if (exp is MethodCallExpression)
            {
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
                string key = string.Join("-", Prefixs) + exp + nodeType;
                var exists = MethodCallExpressionCache.TryGetValue(key, out methodCache);
                if (exists)
                {
                    if (!methodCache.isConstantMethod)
                    {
                        var methodCall = methodCache.CRLExpression.Data as CRLExpression.MethodCallObj;
                        if (methodCall.Args.Count > 0)
                        {
                            for (int i = 0; i < allArguments.Count; i++)
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
                                allConstant = true;
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
                        isConstantMethod = true;
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
                    //like DateTime.Parse("2016-02-11")
                    var method = mcExp.Method;
                    var obj = method.Invoke(null, arguments.ToArray());
                    var exp2= new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Value, Data = obj };
                    var cache = new MethodCallExpressionCacheItem() { CRLExpression = exp2, argsIndex = argsIndex, isConstantMethod = isConstantMethod, isStatic = mcExp.Method.IsStatic };
                    MethodCallExpressionCache[key] = cache;
                    return exp2;
                }
                var methodInfo = new CRLExpression.MethodCallObj() { Args = arguments, ExpressionType = nodeType.Value, MemberName = memberName, MethodName = methodName, MemberQueryName = methodField };
                methodInfo.ReturnType = mcExp.Type;
                #endregion
                var exp4= new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.MethodCall, Data = methodInfo };
                var cache2 = new MethodCallExpressionCacheItem() { CRLExpression = exp4, argsIndex = argsIndex, isConstantMethod = isConstantMethod };
                MethodCallExpressionCache[key] = cache2;
                return exp4;
            }
            else if (exp is ConstantExpression)
            {
                #region 常量
                ConstantExpression cExp = (ConstantExpression)exp;
                object returnValue;
                if (cExp.Value == null)
                {
                    returnValue = "null";
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
            else if (exp is UnaryExpression)
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
                    else if (ue.NodeType== ExpressionType.Convert)
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
        static Dictionary<ExpressionType, string> expressionTypeCache = new Dictionary<ExpressionType, string>();
        public static string ExpressionTypeCast(ExpressionType expType)
        {
            if (expressionTypeCache.Count == 0)
            {
                expressionTypeCache.Add(ExpressionType.And, "&");
                expressionTypeCache.Add(ExpressionType.AndAlso, " AND ");
                expressionTypeCache.Add(ExpressionType.Equal, "=");
                expressionTypeCache.Add(ExpressionType.GreaterThan, ">");
                expressionTypeCache.Add(ExpressionType.GreaterThanOrEqual, ">=");
                expressionTypeCache.Add(ExpressionType.LessThan, "<");
                expressionTypeCache.Add(ExpressionType.LessThanOrEqual, "<=");
                expressionTypeCache.Add(ExpressionType.NotEqual, "<>");
                expressionTypeCache.Add(ExpressionType.Or, "|");
                expressionTypeCache.Add(ExpressionType.OrElse, " OR ");
                expressionTypeCache.Add(ExpressionType.Add, "+");
                expressionTypeCache.Add(ExpressionType.AddChecked, "+");
                expressionTypeCache.Add(ExpressionType.Subtract, "-");
                expressionTypeCache.Add(ExpressionType.SubtractChecked, "-");
                expressionTypeCache.Add(ExpressionType.Multiply, "*");
                expressionTypeCache.Add(ExpressionType.MultiplyChecked, "*");
                expressionTypeCache.Add(ExpressionType.Divide, "/");
                expressionTypeCache.Add(ExpressionType.Not, "!=");
            }
            string type;
            var a = expressionTypeCache.TryGetValue(expType, out type);
            if (a)
            {
                return type;
            }
            throw new InvalidCastException("不支持的运算符" + expType);
            #region old
            switch (expType)
            {
                case ExpressionType.And:
                    return "&";
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Not:
                    return "!=";
                default:
                    throw new InvalidCastException("不支持的运算符" + expType);
            }
            #endregion
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
