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
namespace CRL.LambdaQuery
{
    internal class ExpressionVisitor
    {
        DbContext dbContext;
        DBAdapter.DBAdapterBase __DBAdapter;
        public ExpressionVisitor(DBAdapter.DBAdapterBase _DBAdapter)
        {
            __DBAdapter = _DBAdapter;
            dbContext = _DBAdapter.dbContext;
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
                    throw new Exception("LambdaQuery不支持方法" + method.MethodName);
                }
                int newParIndex = parIndex;
                par = dic[method.MethodName](method, ref newParIndex, AddParame);
                parIndex = newParIndex;
            }

            //字段会返回替换符
            bool needPar = par.IndexOf("{") <= -1;//是否需要参数化处理
            if (!needPar)
            {
                par = par.Replace("{VirtualField}", "");
                par1.Data = par;
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
                    QueryParames.Add("parame" + parIndex, par);
                    par = "@parame" + parIndex;
                    parIndex += 1;
                }
            }
            par1.Data = par;
            return par1;
        }
        CRLExpression.CRLExpression BinaryExpressionHandler(Expression left, Expression right, ExpressionType type)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            var leftPar = RouteExpressionHandler(left);
            string typeStr = ExpressionTypeCast(type);

            var rightPar = RouteExpressionHandler(right);
            string typeStr2;
            leftPar = DealParame(leftPar, typeStr, out typeStr2);
            sb.Append(leftPar.Data);
            rightPar = DealParame(rightPar, typeStr, out typeStr2);
            sb.Append(typeStr2);
            sb.Append(rightPar.Data);
            sb.Append(")");
            //return sb.ToString();
            var types = new ExpressionType[] { ExpressionType.Equal, ExpressionType.GreaterThan, ExpressionType.GreaterThanOrEqual, ExpressionType.LessThan, ExpressionType.LessThanOrEqual, ExpressionType.NotEqual };
            var isBinary = types.Contains(type);
            var e = new CRLExpression.CRLExpression() { ExpressionType = type.ToString(), Left = leftPar, Right = rightPar, Type = isBinary ? CRLExpression.CRLExpressionType.Binary : CRLExpression.CRLExpressionType.Tree };
            e.SqlOut = sb.ToString();
            e.Data = e.SqlOut;
            return e;
        }

        public CRLExpression.CRLExpression RouteExpressionHandler(Expression exp, ExpressionType? nodeType = null)
        {
            //todo 解析不了不带运算符的一元运算 如 b=>b.IsNew
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
                if (mExp.Expression != null && mExp.Expression.NodeType == ExpressionType.Parameter) //like b.Name==b.Name1 或b.Name
                {
                    var fieldName = mExp.Member.Name;
                    var type = mExp.Expression.Type;
                    var filed = TypeCache.GetProperties(type, true)[fieldName];
                    if (filed == null)
                    {
                        throw new Exception("类型 " + type.Name + "." + fieldName + " 不是数据库字段,请检查查询条件");
                    }
                    if (!string.IsNullOrEmpty(filed.VirtualField))//按虚拟字段
                    {
                        //如果没有使用$前辍
                        if (!filed.VirtualField.Contains("$"))
                        {
                            filed.VirtualField = "{VirtualField}" + filed.VirtualField;
                        }
                        //return filed.VirtualField;
                        return new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Name, Data = filed.VirtualField };
                    }
                    var field = Base.FormatFieldPrefix(__DBAdapter, type, filed.MappingName);//格式化为别名
                    //return field;
                    return new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Name, Data = field };
                }
                #endregion
                var obj = GetParameExpressionValue(mExp);
                if (obj is Enum)
                {
                    obj = (int)obj;
                }
                else if (obj is Boolean)//sql2000需要转换
                {
                    obj = Convert.ToInt32(obj);
                }
                //return obj + "";
                return new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Value, Data = obj };
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
                
                if (mcExp.Object is MemberExpression)
                {
                    var mExp = mcExp.Object as MemberExpression;
                    if (mExp.Expression.NodeType != ExpressionType.Parameter)
                    {
                        //not like b.BarCode.Contains("abc")
                        //按变量或常量编译值
                        var obj = GetParameExpressionValue(exp);
                        //return obj + "";
                        return new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Value, Data = obj };
                    }
                }
                else if (mcExp.Object is ConstantExpression)
                {
                    //var cExp = mcExp.Object as ConstantExpression;
                    //like b.BarCode == aa()
                    var obj = GetParameExpressionValue(exp);
                    //return obj + "";
                    return new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Value, Data = obj };
                }
                //var _DBAdapter = DBAdapter.DBAdapterBase.GetDBAdapterBase(dbContext);
                //var methodAnalyze = new CRL.LambdaQuery.MethodAnalyze(_DBAdapter);

                #region 方法
                //请扩展ExtensionMethod的方法
                string methodName = mcExp.Method.Name;
                parIndex += 1;
                //var dic = MethodAnalyze.GetMethos(_DBAdapter);
                //if (!dic.ContainsKey(methodName))
                //{
                //    //return Expression.Lambda(exp).Compile().DynamicInvoke() + "";
                //    throw new Exception("LambdaQuery不支持方法" + mcExp.Method.Name);
                //}
                string field = "";
                #region par
                List<object> args = new List<object>();
                if (mcExp.Object == null)
                {
                    field = RouteExpressionHandler(mcExp.Arguments[0]).Data.ToString();
                }
                else
                {
                    field = mcExp.Object.ToString().Split('.')[1];

                    var mExpression = mcExp.Object as MemberExpression;
                    var type = mExpression.Expression.Type;
                    var filed2 = TypeCache.GetProperties(type, true)[field];
                    field = Base.FormatFieldPrefix(__DBAdapter, type, filed2.MappingName);
                    if (mcExp.Arguments.Count > 0)
                    {
                        var obj = GetParameExpressionValue(mcExp.Arguments[0]);
                        args.Add(obj);
                    }
                }
                
                if (mcExp.Arguments.Count > 1)
                {
                    var obj = GetParameExpressionValue(mcExp.Arguments[1]);
                    args.Add(obj);
                }
                if (mcExp.Arguments.Count > 2)
                {
                    var obj = GetParameExpressionValue(mcExp.Arguments[2]);
                    args.Add(obj);
                }
                if (mcExp.Arguments.Count > 3)
                {
                    var obj = GetParameExpressionValue(mcExp.Arguments[3]);
                    args.Add(obj);
                }
                #endregion
                //int newParIndex = parIndex;
                if (nodeType == null)
                {
                    nodeType = ExpressionType.Equal;
                }
                //var result = dic[methodName](field, nodeType.Value, ref newParIndex, AddParame, args.ToArray());
                //parIndex = newParIndex;
                //return result;
                var methodInfo = new CRLExpression.MethodCallObj() { Args = args, ExpressionType = nodeType.Value, MemberName = field.Substring(field.LastIndexOf("}") + 1), MethodName = methodName, MemberQueryName = field };
                methodInfo.ReturnType = mcExp.Type;
                return new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.MethodCall, Data = methodInfo };
                #endregion
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
                return new CRLExpression.CRLExpression() { Type = CRLExpression.CRLExpressionType.Value, Data = returnValue };
                #endregion
            }
            else if (exp is UnaryExpression)
            {
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
                throw new Exception("未处理的一元运算" + ue.NodeType);
            }
            else
            {
                throw new Exception("不支持此语法解析:" + exp);
            }
        }
        void AddParame(string name, object value)
        {
            QueryParames.Add(name, value);
            //parIndex += 1;
        }
        public static string ExpressionTypeCast(ExpressionType expType)
        {
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
        }
        object GetParameExpressionValue(Expression expression)
        {
            //只能处理常量
            if (expression is ConstantExpression)
            {
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
                        return new ExpressionValueObj { Value = Base.FormatFieldPrefix(__DBAdapter, m.Expression.Type, filed2.MappingName), IsMember = true };
                    }
                }
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
