using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;

namespace CRL.LambdaQuery
{
    public sealed partial class LambdaQuery<T> : LambdaQueryBase where T : IModel, new()
    {
        //internal bool DistinctFields = false;
        /// <summary>
        /// 表示 Distinct字段
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public LambdaQuery<T> DistinctBy<TResult>(Expression<Func<T, TResult>> resultSelector)
        {
            Top(0);
            var fields = GetSelectField(resultSelector.Body, false, typeof(T));
            //DistinctFields = true;
            __FieldFunctionFormat = " DISTINCT {0}";
            __QueryFields = fields;
            return this;
        }
        internal bool distinctCount = false;
        /// <summary>
        /// 表示count Distinct
        /// 结果名为Total
        /// 只能单个字段
        /// </summary>
        /// <returns></returns>
        public LambdaQuery<T> DistinctCount()
        {
            distinctCount = true;
            return this;
        }
    }
}
