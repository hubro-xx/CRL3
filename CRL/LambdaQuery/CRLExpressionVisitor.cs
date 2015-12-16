using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CRL.LambdaQuery
{
    public class CRLExpressionVisitor<T> where T : class, new()
    {
        List<CRLExpression> expressions = new List<CRLExpression>();

        /// <summary>
        /// 返回CRLQueryExpression json格式查询
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string Where(Expression<Func<T, bool>> expression, int pageIndex = 0, int pageSize = 0)
        {
            var response = RouteExpressionHandler(expression.Body);
            return new CRLQueryExpression() { PageSize = pageSize, PageIndex = pageIndex, Type = typeof(T).FullName, Expression = response }.ToJson();
        }

        CRLExpression BinaryExpressionHandler(Expression left, Expression right, ExpressionType type, int level = 1)
        {
            var types = new ExpressionType[] { ExpressionType.Equal, ExpressionType.GreaterThan, ExpressionType.GreaterThanOrEqual, ExpressionType.LessThan, ExpressionType.LessThanOrEqual, ExpressionType.NotEqual };
            var leftPar = RouteExpressionHandler(left);
            var isRight = types.Contains(type);
            var rightPar = RouteExpressionHandler(right, isRight, level);
            var e = new CRLExpression() { ExpressionType = type.ToString(), Left = leftPar, Right = rightPar, Type = isRight ? CRLExpressionType.Binary : CRLExpressionType.Tree };
            return e;
        }
        CRLExpression RouteExpressionHandler(Expression exp, bool isRight = false, int level = 0)
        {
            if (exp is BinaryExpression)
            {
                BinaryExpression be = (BinaryExpression)exp;
                level += 1;
                return BinaryExpressionHandler(be.Left, be.Right, be.NodeType,level);
            }
            else if (exp is MemberExpression)
            {
                MemberExpression mExp = (MemberExpression)exp;
                if (isRight)//按表达式右边值
                {
                    //var obj = Expression.Lambda(mExp).Compile().DynamicInvoke();
                    var obj = LambdaCompileCache.GetExpressionCacheValue(mExp);
                    if (obj is Enum)
                    {
                        obj = (int)obj;
                    }
                    return new CRLExpression() { Type = CRLExpressionType.Value, Data = obj };
                }
                //return mExp.Member.Name;
                return new CRLExpression() { Type = CRLExpressionType.Name, Data = mExp.Member.Name };
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
                //return sb.Length == 0 ? "" : sb.Remove(0, 1).ToString();
                return new CRLExpression() { Type = CRLExpressionType.Value, Data = sb.Length == 0 ? "" : sb.Remove(0, 1).ToString() };
                #endregion
            }
            else if (exp is MethodCallExpression)
            {
                if (isRight)
                {
                    //return Expression.Lambda(exp).Compile().DynamicInvoke() + "";
                    var obj = LambdaCompileCache.GetExpressionCacheValue(exp);
                    return new CRLExpression() { Type = CRLExpressionType.Value, Data = obj };
                }
                //按方法调用
                MethodCallExpression mcExp = (MethodCallExpression)exp;
                string methodName = mcExp.Method.Name;

                string field = "";
                List<object> args = new List<object>();
                if (mcExp.Object == null)
                {
                    field = RouteExpressionHandler(mcExp.Arguments[0]).ToString();
                }
                else
                {
                    field = mcExp.Object.ToString().Split('.')[1];
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
                var methodCall = string.Format("{0}|{1}|{2}", field, methodName, string.Join(",", args));
                return new CRLExpression() { Type = CRLExpressionType.MethodCall, Data = methodCall };
                throw new Exception("暂不支持");
            }
            else if (exp is ConstantExpression)
            {
                #region 常量
                ConstantExpression cExp = (ConstantExpression)exp;
                object value;
                if (cExp.Value == null)
                {
                    value = null;
                }
                else
                {
                    value= cExp.Value.ToString();
                }
                //return value;
                return new CRLExpression() { Type = CRLExpressionType.Value, Data = value };
                #endregion
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                level += 1;
                return RouteExpressionHandler(ue.Operand, isRight, level);
            }
            return null;
        }
        #region 生成表达式
        /// <summary>
        /// 转换为Lambda表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Expression<Func<T, bool>> CreateLambda(CRLExpression expression)
        {
            if (expression.Type == CRLExpressionType.Tree)
            {
                return CreateLambdaTree(expression);
            }
            else if (expression.Type == CRLExpressionType.MethodCall)
            {
                return CreateLambdaMethodCall(expression);
            }
            else
            {
                return CreateLambdaBinary(expression);
            }
        }
        Expression<Func<T, bool>> CreateLambdaTree(CRLExpression expression)
        {
            var expression1 = CreateLambda(expression.Left);
            var expression2 = CreateLambda(expression.Right);
            if (expression.ExpressionType == "AndAlso")
            {
                return expression1.Compose(expression2, Expression.AndAlso);
            }
            else
            {
                return expression1.Compose(expression2, Expression.OrElse);
            }
            return null;
        }
        Expression<Func<T, bool>> CreateLambdaBinary(CRLExpression expression)
        {
            var left = expression.Left;
            var right = expression.Right;
            var creater = new LambdaCreater<T>();
            var type = expression.ExpressionType;
            if (left.Type == CRLExpressionType.MethodCall)//按方法 如 Substring,Indexof
            {
                var dic = new Dictionary<string, BinaryMethodHandler>();
                dic.Add("Equal", creater.Equal);
                dic.Add("NotEqual", creater.NotEqual);
                dic.Add("Greater", creater.Greater);
                dic.Add("Less", creater.Less);
                dic.Add("GreaterThan", creater.GreaterThan);
                dic.Add("LessThan", creater.LessThan);
                if (!dic.ContainsKey(type))
                {
                    throw new Exception("没有对应的运算方法 " + type);
                }
                var left2 = CreateLambdaMethodCall2(left);
                var value = ObjectConvert.ConvertObject(left2.Type, right.Data);
                return dic[type](left2, value);
            }
            else
            {
                var dic = new Dictionary<string, BinaryHandler>();
                dic.Add("Equal", creater.Equal);
                dic.Add("NotEqual", creater.NotEqual);
                dic.Add("Greater", creater.Greater);
                dic.Add("Less", creater.Less);
                dic.Add("GreaterThan", creater.GreaterThan);
                dic.Add("LessThan", creater.LessThan);
                if (!dic.ContainsKey(type))
                {
                    throw new Exception("没有对应的运算方法 " + type);
                }
                return dic[type](left.Data.ToString(), right.Data);
            }
        }
        Expression<Func<T, bool>> CreateLambdaMethodCall(CRLExpression expression)
        {
            var creater = new LambdaCreater<T>();
            var arry = expression.Data.ToString().Split('|');
            var propertyName = arry[0];
            var methodName = arry[1];
            var args = expression.Data.ToString().Substring(propertyName.Length + methodName.Length + 2);
            var dic = new Dictionary<string, MethodHandler>();
            dic.Add("Like", creater.Contains);
            dic.Add("Contains", creater.Contains);
            dic.Add("StartsWith", creater.StartsWith);
            dic.Add("EndsWith", creater.EndsWith);
            if (!dic.ContainsKey(methodName))
            {
                throw new Exception("没有对应的方法 " + methodName);
            }

            return dic[methodName](propertyName, args.Split(','));
        }
        internal MethodCallExpression CreateLambdaMethodCall2(CRLExpression expression)
        {
            var creater = new LambdaCreater<T>();
            var arry = expression.Data.ToString().Split('|');
            var propertyName = arry[0];
            var methodName = arry[1];
            var args = expression.Data.ToString().Substring(propertyName.Length + methodName.Length + 2);
            var dic = new Dictionary<string, MethodResultHandler>();
            dic.Add("Substring", creater.Substring);
            dic.Add("IndexOf", creater.IndexOf);
            if (!dic.ContainsKey(methodName))
            {
                throw new Exception("没有对应的方法 " + methodName);
            }

            return dic[methodName](propertyName, args.Split(','));
        }
        #endregion
        delegate MethodCallExpression MethodResultHandler(string name, params object[] value);
        delegate Expression<Func<T, bool>> MethodHandler(string name, params object[] value);
        delegate Expression<Func<T, bool>> BinaryHandler(string name, object value);
        delegate Expression<Func<T, bool>> BinaryMethodHandler(MethodCallExpression left, object value);
    }
    
}
