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
    /// <typeparam name="TJoinResult"></typeparam>
    public sealed class LambdaQueryViewJoin<T, TJoinResult>
        where T : IModel, new()
    {
        LambdaQueryResultSelect<TJoinResult> resultSelect;
        /// <summary>
        /// 关联查询分支
        /// </summary>
        /// <param name="query"></param>
        /// <param name="_resultSelect"></param>
        internal LambdaQueryViewJoin(LambdaQuery<T> query, LambdaQueryResultSelect<TJoinResult> _resultSelect)
        {
            BaseQuery = query;
            resultSelect = _resultSelect;
        }
        LambdaQuery<T> BaseQuery;

        /// <summary>
        /// 返回强类型结果选择
        /// </summary>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public LambdaQueryResultSelect<TJoinResult2> Select<TJoinResult2>(Expression<Func<T, TJoinResult, TJoinResult2>> resultSelector)
            where TJoinResult2 : class
        {
            var parameters = resultSelector.Parameters.Select(b => b.Type).ToArray();
            var selectField = BaseQuery.GetSelectField(true, resultSelector.Body, false, parameters);
            var resultFields = selectField.fields;
            var prefix1 = BaseQuery.GetPrefix(typeof(TJoinResult));
            var prefix2 = BaseQuery.GetPrefix(resultSelect.InnerType);
            //替换匿名前辍
            foreach (var item in resultFields)
            {
                if (item.QueryFullScript.Contains(prefix1))
                {
                    item.QueryFullScript = item.QueryFullScript.Replace(prefix1, prefix2);
                }
            }
            selectField.queryFieldString = BaseQuery.GetQueryFieldsString(resultFields);
            BaseQuery._CurrentSelectFieldCache = selectField;
            //BaseQuery.__QueryFields = resultFields;
            return new LambdaQueryResultSelect<TJoinResult2>(BaseQuery, resultSelector.Body);
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
            //if (BaseQuery.__QueryFields.Count == 0)
            //{
            //    BaseQuery.SelectAll();
            //}
            if (BaseQuery._CurrentSelectFieldCache==null)
            {
                BaseQuery.SelectAll();
            }
            var parameters = resultSelector.Parameters.Select(b => b.Type).ToArray();
            var resultFields = BaseQuery.GetSelectField(true, resultSelector.Body, true, parameters).fields;
            var prefix1 = BaseQuery.GetPrefix(typeof(TJoinResult));
            var prefix2 = BaseQuery.GetPrefix(resultSelect.InnerType);
            //替换匿名前辍
            foreach (var item in resultFields)
            {
                if (item.QueryFullScript.Contains(prefix1))
                {
                    item.QueryFullScript = item.QueryFullScript.Replace(prefix1, prefix2);
                }
            }

            //BaseQuery.__QueryFields.AddRange(resultFields);
            BaseQuery._CurrentAppendSelectField.AddRange(resultFields);
            return BaseQuery as LambdaQuery<T>;
        }
        /// <summary>
        /// 按TJoin排序
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public LambdaQueryViewJoin<T, TJoinResult> OrderBy<TResult>(Expression<Func<TJoinResult, TResult>> expression, bool desc = true) 
        {
            //var innerType = typeof(TJoin);
            var parameters = expression.Parameters.Select(b => b.Type).ToArray();
            var fields = BaseQuery.GetSelectField(false, expression.Body, false, parameters).fields;
            if (!string.IsNullOrEmpty(BaseQuery.__QueryOrderBy))
            {
                BaseQuery.__QueryOrderBy += ",";
            }
            BaseQuery.__QueryOrderBy += string.Format(" {0} {1}", fields.First().QueryField, desc ? "desc" : "asc");
            return this;
        }
    }
}
