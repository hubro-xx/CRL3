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

namespace CRL.LambdaQuery.CRLExpression
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
            var isBinary = types.Contains(type);
            var rightPar = RouteExpressionHandler(right, level);
            var e = new CRLExpression() { ExpressionType = type.ToString(), Left = leftPar, Right = rightPar, Type = isBinary ? CRLExpressionType.Binary : CRLExpressionType.Tree };
            return e;
        }
        CRLExpression RouteExpressionHandler(Expression exp, int level = 0)
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
                if (mExp.Expression != null && mExp.Expression.NodeType == ExpressionType.Parameter) //like b.Name==b.Name1 或b.Name
                {
                    return new CRLExpression() { Type = CRLExpressionType.Name, Data = mExp.Member.Name };
                }
                //var obj = Expression.Lambda(mExp).Compile().DynamicInvoke();
                var obj = LambdaCompileCache.GetParameExpressionValue(mExp);
                if (obj is Enum)
                {
                    obj = (int)obj;
                }
                return new CRLExpression() { Type = CRLExpressionType.Value, Data = obj };
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
                MethodCallExpression mcExp = (MethodCallExpression)exp;
                if (mcExp.Object is MemberExpression)
                {
                    var mExp = mcExp.Object as MemberExpression;
                    if (mExp.Expression.NodeType != ExpressionType.Parameter)
                    {
                        //not like b.BarCode.Contains("abc")
                        //按变量或常量编译值
                        var obj = LambdaCompileCache.GetParameExpressionValue(exp);
                        return new CRLExpression() { Type = CRLExpressionType.Value, Data = obj };
                    }
                }
                else if (mcExp.Object is ConstantExpression)
                {
                    //var cExp = mcExp.Object as ConstantExpression;
                    //like b.BarCode == aa()
                    var obj = LambdaCompileCache.GetParameExpressionValue(exp);
                    return new CRLExpression() { Type = CRLExpressionType.Value, Data = obj };
                }
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
                    var obj = LambdaCompileCache.GetParameExpressionValue(mcExp.Arguments[0]);
                    args.Add(obj);
                    //args.Add(Expression.Lambda(mcExp.Arguments[0]).Compile().DynamicInvoke());
                }
                if (mcExp.Arguments.Count > 1)
                {
                    var obj = LambdaCompileCache.GetParameExpressionValue(mcExp.Arguments[1]);
                    args.Add(obj);
                    //args.Add(Expression.Lambda(mcExp.Arguments[1]).Compile().DynamicInvoke());
                }
                if (mcExp.Arguments.Count > 2)
                {
                    var obj = LambdaCompileCache.GetParameExpressionValue(mcExp.Arguments[2]);
                    args.Add(obj);
                    //args.Add(Expression.Lambda(mcExp.Arguments[2]).Compile().DynamicInvoke());
                }
                if (mcExp.Arguments.Count > 3)
                {
                    var obj = LambdaCompileCache.GetParameExpressionValue(mcExp.Arguments[3]);
                    args.Add(obj);
                    //args.Add(Expression.Lambda(mcExp.Arguments[3]).Compile().DynamicInvoke());
                }
                var methodCall = string.Format("{0}|{1}|{2}", field, methodName, string.Join(",", args));
                return new CRLExpression() { Type = CRLExpressionType.MethodCall, Data = methodCall };
                throw new NotSupportedException("暂不支持");
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
                return RouteExpressionHandler(ue.Operand, level);
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
                //解析一个表达式树
                return CreateLambdaTree(expression);
            }
            else if (expression.Type == CRLExpressionType.MethodCall)
            {
                //方法
                return CreateLambdaMethodCall(expression);
            }
            else
            {
                //二元运算
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

            var dic = new Dictionary<string, ExpressionHandler>();
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

            if (left.Type == CRLExpressionType.MethodCall)//按方法运算 如 b.NameSubstring,Indexof(0,1)=="123"
            {
                //按属性的子方法
                var left2 = CreateLambdaMethodCall2(left);
                var value = ObjectConvert.ConvertObject(left2.Type, right.Data);
                return dic[type](left2, value);
            }
            else//按属性运算 如b.Id==1
            {
                //todo 暂不支持属性间比较  b=>b.Id>b.Number
                var member = creater.CreatePropertyExpression(left.Data.ToString());
                return dic[type](member, right.Data);
            }
        }
        Expression<Func<T, bool>> CreateLambdaMethodCall(CRLExpression expression)
        {
            //表示方法调用
            var creater = new LambdaCreater<T>();
            var arry = expression.Data.ToString().Split('|');
            var propertyName = arry[0];
            var methodName = arry[1];
            var args = expression.Data.ToString().Substring(propertyName.Length + methodName.Length + 2);
            var dic = new Dictionary<string, MethodHandler>();
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
            //表示二元运算的方法
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
        delegate Expression<Func<T, bool>> ExpressionHandler(Expression left, object value);
    }
    
}
