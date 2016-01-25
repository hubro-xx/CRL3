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
        public static object GetExpressionCacheValue(Expression expression)
        {
            //只能处理常量
            if (expression is ConstantExpression)
            {
                ConstantExpression cExp = (ConstantExpression)expression;
                return cExp.Value;
            }
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }
    }
}
