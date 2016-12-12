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
using System.Threading.Tasks;

namespace CRL.LambdaQuery.CRLExpression
{
    /// <summary>
    /// 表示一个方法调用
    /// </summary>
    internal class MethodCallObj
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string MemberName;
        /// <summary>
        /// SQL查询名
        /// </summary>
        public string MemberQueryName;
        public string MethodName;
        /// <summary>
        /// 返回类型
        /// </summary>
        public Type ReturnType;
        /// <summary>
        /// 二元运算类型
        /// </summary>
        public ExpressionType ExpressionType;
        public List<object> Args = null;
    }

}
