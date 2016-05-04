/**
* CRL 快速开发框架 V3.1
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
using System.Threading.Tasks;

namespace CRL.LambdaQuery
{
    internal class LambdaCompileCache
    {
        //static Dictionary<string, object> ResultCache = new Dictionary<string, object>();
        //static object lockObj = new object();
        /// <summary>
        /// 获取用作参数的表达式值
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static object GetParameExpressionValue(Expression expression)
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
                        return new ExpressionValueObj { Value = Base.FormatFieldPrefix(m.Expression.Type, m.Member.Name), IsMember = true };
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
