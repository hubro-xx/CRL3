using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery
{
    /// <summary>
    /// 关联视图查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TJoin"></typeparam>
    public sealed class LambdaQueryViewJoin<T, TJoin, TJoinResult>
        where T : IModel, new()
    {
        /// <summary>
        /// 关联查询分支
        /// </summary>
        /// <param name="query"></param>
        /// <param name="resultSelect"></param>
        internal LambdaQueryViewJoin(LambdaQuery<T> query, LambdaQueryResultSelect<TJoin, TJoinResult> resultSelect)
        {
            BaseQuery = query;
        }
        LambdaQuery<T> BaseQuery;

        /// <summary>
        /// 返回强类型结果选择
        /// </summary>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public LambdaQueryResultSelect<TJoinResult, TJoinResult2> Select<TJoinResult2>(Expression<Func<T, TJoinResult, TJoinResult2>> resultSelector)
            where TJoinResult2 : class
        {
            var resultFields = BaseQuery.GetSelectField(true, resultSelector.Body, false, typeof(T), typeof(TJoin));
            var prefix1 = BaseQuery.GetPrefix(typeof(TJoinResult));
            var prefix2 = BaseQuery.GetPrefix(typeof(TJoin));
            foreach (var item in resultFields)
            {
                if (item.QueryFullScript.StartsWith(prefix1))
                {
                    item.QueryFullScript = item.QueryFullScript.Replace(prefix1, prefix2);
                }
            }
            BaseQuery.__QueryFields = resultFields;
            return new LambdaQueryResultSelect<TJoinResult, TJoinResult2>(BaseQuery, resultSelector.Body);
        }
        /// <summary>
        /// 选择TJoin关联值到对象内部索引
        /// 可调用多次,不要重复
        /// </summary>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public LambdaQuery<T> SelectAppendValue<TJoinResult2>(Expression<Func<TJoinResult, TJoinResult2>> resultSelector)
        {
            //var innerType = typeof(TJoin);
            if (BaseQuery.__QueryFields.Count == 0)
            {
                BaseQuery.SelectAll();
            }
            var resultFields = BaseQuery.GetSelectField(true, resultSelector.Body, true, typeof(TJoin));
            var prefix1 = BaseQuery.GetPrefix(typeof(TJoinResult));
            var prefix2 = BaseQuery.GetPrefix(typeof(TJoin));
            foreach (var item in resultFields)
            {
                if (item.QueryFullScript.StartsWith(prefix1))
                {
                    item.QueryFullScript = item.QueryFullScript.Replace(prefix1, prefix2);
                }
            }

            BaseQuery.__QueryFields.AddRange(resultFields);
            return BaseQuery as LambdaQuery<T>;
        }
        /// <summary>
        /// 按TJoin排序
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public LambdaQueryViewJoin<T, TJoin, TJoinResult> OrderBy<TResult>(Expression<Func<TJoin, TResult>> expression, bool desc = true) 
        {
            //var innerType = typeof(TJoin);
            var fields = BaseQuery.GetSelectField(false, expression.Body, false, typeof(T), typeof(TJoin));
            if (!string.IsNullOrEmpty(BaseQuery.__QueryOrderBy))
            {
                BaseQuery.__QueryOrderBy += ",";
            }
            BaseQuery.__QueryOrderBy += string.Format(" {0} {1}", fields.First().QueryField, desc ? "desc" : "asc");
            return this;
        }
    }
}
