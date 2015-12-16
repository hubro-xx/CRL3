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
        public ExpressionVisitor(DbContext _dbContext)
        {
            dbContext = _dbContext;
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
        string DealParame(string par, string typeStr,out string typeStr2)
        {
            typeStr2 = typeStr;
            //字段会返回替换符
            bool needPar = par.IndexOf("{") <= -1;//是否需要参数化处理
            if (!needPar)
            {
                par = par.Replace("{VirtualField}", "");
                return par;
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
            return par;
        }
        public string BinaryExpressionHandler(Expression left, Expression right, ExpressionType type)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            string leftPar = RouteExpressionHandler(left);
            string typeStr = ExpressionTypeCast(type);

            string rightPar = RouteExpressionHandler(right);
            string typeStr2;
            leftPar = DealParame(leftPar, typeStr, out typeStr2);
            sb.Append(leftPar);
            rightPar = DealParame(rightPar, typeStr, out typeStr2);
            sb.Append(typeStr2);
            sb.Append(rightPar);
            sb.Append(")");
            return sb.ToString();
        }
        
        public string RouteExpressionHandler(Expression exp, ExpressionType nodeType= ExpressionType.Equal)
        {
            if (exp is BinaryExpression)
            {
                BinaryExpression be = (BinaryExpression)exp;
                return BinaryExpressionHandler(be.Left, be.Right, be.NodeType);
            }
            else if (exp is MemberExpression)
            {
                //区分 属性表达带替换符{0} 变量值不带
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
                        return filed.VirtualField;
                    }
                    var field = "{" + type.FullName + "}" + fieldName;//格式化为别名
                    if (nodeType == ExpressionType.Not)//like b=!b.IsNew
                    {
                        field += ExpressionTypeCast(nodeType) +1;
                    }
                    return field;
                }
                var obj = LambdaCompileCache.GetExpressionCacheValue(mExp);
                if (obj is Enum)
                {
                    obj = (int)obj;
                }
                return obj + "";
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
                return sb.Length == 0 ? "" : sb.Remove(0, 1).ToString();
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
                        var obj = LambdaCompileCache.GetExpressionCacheValue(exp);
                        return obj + "";
                    }
                }
                else if (mcExp.Object is ConstantExpression)
                {
                    //var cExp = mcExp.Object as ConstantExpression;
                    //like b.BarCode == aa()
                    var obj = LambdaCompileCache.GetExpressionCacheValue(exp);
                    return obj + "";
                }
                var _DBAdapter = DBAdapter.DBAdapterBase.GetDBAdapterBase(dbContext);
                //var methodAnalyze = new CRL.LambdaQuery.MethodAnalyze(_DBAdapter);
                var dic = MethodAnalyze.GetMethos(_DBAdapter);
                #region 方法
                //请扩展ExtensionMethod的方法
                string methodName = mcExp.Method.Name;
                parIndex += 1;
                if (!dic.ContainsKey(methodName))
                {
                    //return Expression.Lambda(exp).Compile().DynamicInvoke() + "";
                    throw new Exception("LambdaQuery不支持方法" + mcExp.Method.Name);
                }
                string field = "";
                #region par
                List<object> args = new List<object>();
                if (mcExp.Object == null)
                {
                    field = RouteExpressionHandler(mcExp.Arguments[0]);
                }
                else
                {
                    field = mcExp.Object.ToString().Split('.')[1];
                    var mExpression = mcExp.Object as MemberExpression;
                    var type = mExpression.Expression.Type;
                    field = "{" + type.FullName + "}" + field;
                    var obj = LambdaCompileCache.GetExpressionCacheValue(mcExp.Arguments[0]);
                    args.Add(obj);
                    //args.Add(Expression.Lambda(mcExp.Arguments[0]).Compile().DynamicInvoke());
                }
                
                if (mcExp.Arguments.Count > 1)
                {
                    var obj = LambdaCompileCache.GetExpressionCacheValue(mcExp.Arguments[1]);
                    args.Add(obj);
                    //args.Add(Expression.Lambda(mcExp.Arguments[1]).Compile().DynamicInvoke());
                }
                if (mcExp.Arguments.Count > 2)
                {
                    var obj = LambdaCompileCache.GetExpressionCacheValue(mcExp.Arguments[2]);
                    args.Add(obj);
                    //args.Add(Expression.Lambda(mcExp.Arguments[2]).Compile().DynamicInvoke());
                }
                if (mcExp.Arguments.Count > 3)
                {
                    var obj = LambdaCompileCache.GetExpressionCacheValue(mcExp.Arguments[3]);
                    args.Add(obj);
                    //args.Add(Expression.Lambda(mcExp.Arguments[3]).Compile().DynamicInvoke());
                }
                #endregion
                int newParIndex = parIndex;
                var result = dic[methodName](field, nodeType, ref newParIndex, AddParame, args.ToArray());
                parIndex = newParIndex;
                return result;
                #endregion
            }
            else if (exp is ConstantExpression)
            {
                #region 常量
                ConstantExpression cExp = (ConstantExpression)exp;
                if (cExp.Value == null)
                    return "null";
                else
                {
                    if (cExp.Value is Boolean)
                    {
                        return Convert.ToInt32(cExp.Value).ToString();
                    }
                    else if (cExp.Value is Enum)
                    {
                        return Convert.ToInt32(cExp.Value).ToString();
                    }
                    return cExp.Value.ToString();
                }
                #endregion
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                return RouteExpressionHandler(ue.Operand, ue.NodeType);
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
        public string ExpressionTypeCast(ExpressionType expType)
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
                    throw new InvalidCastException("不支持的运算符");
            }
        }

    }
}
