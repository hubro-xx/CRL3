/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System.Collections.Generic;
using System.Linq.Expressions;
namespace CRL.LambdaQuery
{
    /// <summary>
    /// 参数重绑定操作
    /// </summary>
    internal class ParameterRebinder : System.Linq.Expressions.ExpressionVisitor
    {
        /// <summary>
        /// 参数字典
        /// </summary>
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        /// <summary>
        /// 初始化参数重绑定操作
        /// </summary>
        /// <param name="map">参数字典</param>
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// 替换参数
        /// </summary>
        /// <param name="map">参数字典</param>
        /// <param name="exp">表达式</param>
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        /// <summary>
        /// 访问参数
        /// </summary>
        /// <param name="parameterExpression">参数</param>
        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            ParameterExpression replacement;
            if (_map.TryGetValue(parameterExpression, out replacement))
                parameterExpression = replacement;
            return base.VisitParameter(parameterExpression);
        }
    }
}
