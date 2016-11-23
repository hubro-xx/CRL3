using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery
{
    /// <summary>
    /// 返回强类型选择结果查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public sealed class LambdaQueryResultSelect<T, TResult>
    {

        Expression resultSelectorBody;
        internal LambdaQueryResultSelect(LambdaQueryBase query, Expression _resultSelectorBody)
        {
            resultSelectorBody=_resultSelectorBody;
            BaseQuery = query;

        }
        internal LambdaQueryBase BaseQuery;
        /// <summary>
        /// 联合查询
        /// 会清除父查询的排序
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult2"></typeparam>
        /// <param name="view2"></param>
        /// <param name="unionType"></param>
        /// <returns></returns>
        public LambdaQueryResultSelect<T, TResult> Union<T2, TResult2>(LambdaQueryResultSelect<T2, TResult2> view2, UnionType unionType = UnionType.UnionAll)
        {
            BaseQuery.__QueryOrderBy = "";//清除OrderBy
            BaseQuery.AddUnion(view2.BaseQuery, unionType);
            return this;
        }
        /// <summary>
        /// 设置排序
        /// </summary>
        /// <typeparam name="TResult2"></typeparam>
        /// <param name="expression"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public LambdaQueryResultSelect<T, TResult> OrderBy<TResult2>(Expression<Func<TResult, TResult2>> expression, bool desc = true)
        {
            var fields = BaseQuery.GetSelectField(false, expression.Body, false, typeof(T));
            var orderBy = string.Format(" {0} {1}", fields.First().QueryField, desc ? "desc" : "asc");
            if (!string.IsNullOrEmpty(BaseQuery.__QueryOrderBy))
            {
                BaseQuery.__QueryOrderBy += ",";
            }
            BaseQuery.__QueryOrderBy += orderBy;
            return this;
        }
        /// <summary>
        /// 返回自定义类型
        /// </summary>
        /// <typeparam name="TResult2"></typeparam>
        /// <returns></returns>
        public List<TResult2> ToList<TResult2>()
        {
            var db = DBExtendFactory.CreateDBExtend(BaseQuery.__DbContext);
            return db.QueryResult<TResult2>(BaseQuery);
        }
        /// <summary>
        /// 返回筛选类型
        /// </summary>
        /// <returns></returns>
        public List<TResult> ToList()
        {
            var db = DBExtendFactory.CreateDBExtend(BaseQuery.__DbContext);
            if (resultSelectorBody is NewExpression)
            {
                var newExpression = resultSelectorBody as NewExpression;
                return db.QueryResult<TResult>(BaseQuery, newExpression);
            }
            throw new CRLException("ToList不支持此表达式 " + resultSelectorBody);
        }
    }
}
