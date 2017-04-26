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
using System.Reflection;
using System.Text;

namespace CRL.LambdaQuery.CRLExpression
{
    public class CRLExpressionVisitor<T> where T : class, new()
    {
        LambdaCreater<T> creater;
        public CRLExpressionVisitor()
        {
            //由于变量作用域的问题,只实例化一次,并且不能用缓存
            creater = new LambdaCreater<T>();
        }
        List<CRLExpression> expressions = new List<CRLExpression>();

        /// <summary>
        /// 返回CRLQueryExpression json格式查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public string Where(Expression<Func<T, bool>> expression, int pageIndex = 0, int pageSize = 0)
        {
            var response = RouteExpressionHandler(expression.Body);
            return new CRLQueryExpression() { Size = pageSize, Page = pageIndex, Type = typeof(T).FullName, Exp = response }.ToJson();
        }

        CRLExpression BinaryExpressionHandler(Expression left, Expression right, ExpressionType type, int level = 1)
        {
            var types = new ExpressionType[] { ExpressionType.Equal, ExpressionType.GreaterThan, ExpressionType.GreaterThanOrEqual, ExpressionType.LessThan, ExpressionType.LessThanOrEqual, ExpressionType.NotEqual };
            var leftPar = RouteExpressionHandler(left);
            var isBinary = types.Contains(type);
            var rightPar = RouteExpressionHandler(right);
            var e = new CRLExpression() { ExpType = type.ToString(), Left = leftPar, Right = rightPar, Type = isBinary ? CRLExpressionType.Binary : CRLExpressionType.Tree };
            return e;
        }
        
        #region new 
        CRLExpression RouteExpressionHandler(Expression exp, ExpressionType? nodeType = null)
        {
            if (exp is BinaryExpression)
            {
                BinaryExpression be = (BinaryExpression)exp;
                return BinaryExpressionHandler(be.Left, be.Right, be.NodeType);
            }
            if (exp is MemberExpression)
            {
                return MemberExpressionHandler(exp, nodeType);
            }
            else if (exp is ConstantExpression)
            {
                return ConstantExpressionHandler(exp, nodeType);
            }
            else if (exp is MethodCallExpression)
            {
                return MethodCallExpressionHandler(exp, nodeType);
            }
            else if (exp is UnaryExpression)
            {
                return UnaryExpressionHandler(exp, nodeType);
            }
            else if (exp is NewArrayExpression)
            {
                return NewArrayExpressionHandler(exp, nodeType);
            }
            else
            {
                throw new CRLException("不支持此语法解析:" + exp);
            }
        }
        #region 按类型解析
        CRLExpression MemberExpressionHandler(Expression exp, ExpressionType? nodeType = null)
        {
            MemberExpression mExp = (MemberExpression)exp;
            if (mExp.Expression != null && mExp.Expression.NodeType == ExpressionType.Parameter) //like b.Name==b.Name1 或b.Name
            {
                return new CRLExpression() { Type = CRLExpressionType.Name, Data = mExp.Member.Name };
            }
            //var obj = Expression.Lambda(mExp).Compile().DynamicInvoke();
            var obj = ConstantValueVisitor.GetParameExpressionValue(mExp);
            if (obj is Enum)
            {
                obj = (int)obj;
            }
            return new CRLExpression() { Type = CRLExpressionType.Value, Data = obj };
        }
        CRLExpression NewArrayExpressionHandler(Expression exp, ExpressionType? nodeType = null, bool firstLevel = false)
        {
            #region 数组
            NewArrayExpression naExp = (NewArrayExpression)exp;
            StringBuilder sb = new StringBuilder();
            foreach (Expression expression in naExp.Expressions)
            {
                sb.AppendFormat(",{0}", RouteExpressionHandler(expression));
            }
            var str = sb.Length == 0 ? "" : sb.Remove(0, 1).ToString();
            return new CRLExpression() { Type = CRLExpressionType.Value, Data = str };
            #endregion
        }
        CRLExpression MethodCallExpressionHandler(Expression exp, ExpressionType? nodeType = null, bool firstLevel = false)
        {
            MethodCallExpression mcExp = (MethodCallExpression)exp;
            if (mcExp.Object is MemberExpression)
            {
                var mExp = mcExp.Object as MemberExpression;
                if (mExp.Expression.NodeType != ExpressionType.Parameter)
                {
                    //not like b.BarCode.Contains("abc")
                    //按变量或常量编译值
                    var obj = ConstantValueVisitor.GetParameExpressionValue(exp);
                    return new CRLExpression() { Type = CRLExpressionType.Value, Data = obj };
                }
            }
            else if (mcExp.Object is ConstantExpression)
            {
                //var cExp = mcExp.Object as ConstantExpression;
                //like b.BarCode == aa()
                var obj = ConstantValueVisitor.GetParameExpressionValue(exp);
                return new CRLExpression() { Type = CRLExpressionType.Value, Data = obj };
            }
            string methodName = mcExp.Method.Name;

            string field = "";
            List<object> args = new List<object>();
            if (mcExp.Object == null)
            {
                field = RouteExpressionHandler(mcExp.Arguments[0]).Data.ToString();
            }
            else
            {
                field = mcExp.Object.ToString().Split('.')[1];
                if (mcExp.Arguments.Count > 0)
                {
                    var obj = ConstantValueVisitor.GetParameExpressionValue(mcExp.Arguments[0]);
                    args.Add(obj);
                }
                //args.Add(Expression.Lambda(mcExp.Arguments[0]).Compile().DynamicInvoke());
            }
            if (mcExp.Arguments.Count > 1)
            {
                for (int i = 1; i < mcExp.Arguments.Count; i++)
                {
                    var obj = ConstantValueVisitor.GetParameExpressionValue(mcExp.Arguments[i]);
                    args.Add(obj);
                }
            }
            if (methodName == "Parse")
            {
                methodName = string.Format("To{0}", mcExp.Method.DeclaringType.Name);
            }
            var methodCall = string.Format("{0}|{1}|{2}", field, methodName, string.Join(",", args));
            return new CRLExpression() { Type = CRLExpressionType.MethodCall, Data = methodCall };
            throw new NotSupportedException("暂不支持");
        }
        CRLExpression ConstantExpressionHandler(Expression exp, ExpressionType? nodeType = null, bool firstLevel = false)
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
            return new CRLExpression() { Type = CRLExpressionType.Value, Data = returnValue, IsConstantValue = true };
            #endregion
        }
        CRLExpression UnaryExpressionHandler(Expression exp, ExpressionType? nodeType = null, bool firstLevel = false)
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
        #endregion
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
                return CreateLambdaBinary(expression) as Expression<Func<T, bool>>;
            }
        }
        Expression<Func<T, bool>> CreateLambdaTree(CRLExpression expression)
        {
            var expression1 = CreateLambda(expression.Left);
            var expression2 = CreateLambda(expression.Right);
            switch (expression.ExpType)
            {
                case "AndAlso":
                    return expression1.Compose(expression2, Expression.AndAlso);
                case "OrElse":
                    return expression1.Compose(expression2, Expression.OrElse);
                case "And":
                    return expression1.Compose(expression2, Expression.And);
                case "Or":
                    return expression1.Compose(expression2, Expression.Or);
            }
            //if (expression.ExpressionType == "AndAlso")
            //{
            //    return expression1.Compose(expression2, Expression.AndAlso);
            //}
            //else
            //{
            //    return expression1.Compose(expression2, Expression.OrElse);
            //}
            return null;
        }
        Expression CreateLambdaBinary(CRLExpression expression)
        {
            var left = expression.Left;
            var right = expression.Right;
            Expression exp;
            object value;
            var type = expression.ExpType;
            if (left.Type == CRLExpressionType.MethodCall)//按方法运算 如 b.NameSubstring,Indexof(0,1)=="123"
            {
                //按属性的子方法
                exp = CreateLambdaMethodCall2(left);
                value = ObjectConvert.ConvertObject(exp.Type, right.Data);
            }
            else if (left.Type == CRLExpressionType.Tree)
            {
                exp = CreateLambdaBinary(left);//todo 暂时在左边
                value = right.Data;
                //throw new CRLException("不支持的运算 "+ left.ExpressionType);
            }
            else//按属性运算 如b.Id==1
            {
                exp = creater.CreatePropertyExpression(left.Data.ToString());
                value = right.Data;
            }

            switch (type)
            {
                case "Equal":
                    return creater.Equal(exp, value);
                case "NotEqual":
                    return creater.NotEqual(exp, value);
                case "Greater":
                    return creater.Greater(exp, value);
                case "Less":
                    return creater.Less(exp, value);
                case "GreaterThan":
                    return creater.GreaterThan(exp, value);
                case "LessThan":
                    return creater.LessThan(exp, value);
                case "And":
                    return creater.And(exp, value);
                case "Or":
                    return creater.Or(exp, value);
                default:
                    throw new CRLException("没有对应的运算方法 " + type);
            }  
        }
        static Dictionary<string, MethodHandler> CreateLambdaMethodCallCache = new Dictionary<string, MethodHandler>();
        Expression<Func<T, bool>> CreateLambdaMethodCall(CRLExpression expression)
        {
            //表示方法调用
            var arry = expression.Data.ToString().Split('|');
            var propertyName = arry[0];
            var methodName = arry[1];
            var args = expression.Data.ToString().Substring(propertyName.Length + methodName.Length + 2);
            var value = args.Split(',');
            if (args == "")
            {
                value = new string[] { };
            }
            //if (CreateLambdaMethodCallCache.Count == 0)
            //{
            //    CreateLambdaMethodCallCache.Add("Contains", creater.Contains);
            //    CreateLambdaMethodCallCache.Add("StartsWith", creater.StartsWith);
            //    CreateLambdaMethodCallCache.Add("EndsWith", creater.EndsWith);
            //    CreateLambdaMethodCallCache.Add("IsNullOrEmpty", creater.EndsWith);
            //}
            MethodHandler method;
            switch (methodName)
            {
                case "Contains":
                    method = creater.Contains;
                    break;
                case "StartsWith":
                    method = creater.StartsWith;
                    break;
                case "EndsWith":
                    method = creater.EndsWith;
                    break;
                case "IsNullOrEmpty":
                    method = creater.IsNullOrEmpty;
                    break;
                case "Equals":
                    method = creater.Equals;
                    break;
                default:
                    throw new CRLException("没有对应的方法 " + methodName);
            }
            //var a = CreateLambdaMethodCallCache.TryGetValue(methodName, out method);
            ////todo 更多方法解析
            //if (!a)
            //{
            //    throw new CRLException("没有对应的方法 " + methodName);
            //}
            return method(propertyName, value);
        }
        static Dictionary<string, MethodInfo> CreateLambdaMethodCall2Cahce = new Dictionary<string, MethodInfo>();
        internal MethodCallExpression CreateLambdaMethodCall2(CRLExpression expression)
        {
            //表示二元运算的方法
            var arry = expression.Data.ToString().Split('|');
            var propertyName = arry[0];
            var methodName = arry[1];
            var args = expression.Data.ToString().Substring(propertyName.Length + methodName.Length + 2);
            string[] value = args.Split(',');
            if(args=="")
            {
                value = new string[] { };
            }
            MethodResultHandler method;
            switch (methodName)
            {
                case "Substring":
                    method = creater.Substring;
                    break;
                case "IndexOf":
                    method = creater.IndexOf;
                    break;
                case "ToString":
                    method = creater.ToString;
                    break;
                case "ToInt32":
                    method = creater.ToInt32;
                    break;
                case "ToDecimal":
                    method = creater.ToDecimal;
                    break;
                case "ToDouble":
                    method = creater.ToDouble;
                    break;
                case "ToBoolean":
                    method = creater.ToBoolean;
                    break;
                case "ToDateTime":
                    method = creater.ToDateTime;
                    break;
                case "ToInt16":
                    method = creater.ToInt16;
                    break;
                case "ToSingle":
                    method = creater.ToSingle;
                    break;
                case "ToLower":
                    method = creater.ToLower;
                    break;
                case "ToUpper":
                    method = creater.ToUpper;
                    break;
                default:
                    throw new CRLException("没有对应的方法 " + methodName);
            }
            return method(propertyName, value);
        }
        #endregion
        delegate MethodCallExpression MethodResultHandler(string name, params object[] value);
        delegate Expression<Func<T, bool>> MethodHandler(string name, params object[] value);
        delegate Expression<Func<T, bool>> ExpressionHandler(Expression left, object value);
    }
    
}
