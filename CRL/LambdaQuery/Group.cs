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
using System.Text;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using CRL.LambdaQuery;
using System.Text.RegularExpressions;

namespace CRL.LambdaQuery
{
    public sealed partial class LambdaQuery<T> : LambdaQueryBase where T : IModel, new()
    {
        #region group
        /// <summary>
        /// 设置GROUP字段
        /// </summary>
        /// <param name="resultSelector">like b=>new{b.Name,b.Id}</param>
        /// <returns></returns>
        public LambdaQuery<T> GroupBy(Expression<Func<T, object>> resultSelector)
        {
            var fields = GetSelectField(resultSelector.Body, false, typeof(T));
            __GroupFields = fields;
            //CompileSp = true;
            return this;
        }
        

        /// <summary>
        /// 设置group having条件
        /// like b => b.Number.SUM() > 1
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> GroupHaving(Expression<Func<T, bool>> expression)
        {
            string condition = FormatExpression(expression.Body);
            Having += string.IsNullOrEmpty(Having) ? condition : " and " + condition;
            return this;
        }

        #endregion
    }
}
