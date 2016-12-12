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
    /// <typeparam name="TResult"></typeparam>
    public sealed class LambdaQueryResultSelect<TResult>
    {
        Expression resultSelectorBody;
        internal Type InnerType
        {
            get
            {
                return BaseQuery.__MainType;
            }
        }
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
        /// <typeparam name="TResult2"></typeparam>
        /// <param name="resultSelect"></param>
        /// <param name="unionType"></param>
        /// <returns></returns>
        public LambdaQueryResultSelect<TResult> Union<TResult2>(LambdaQueryResultSelect<TResult2> resultSelect, UnionType unionType = UnionType.UnionAll)
        {
            BaseQuery.__QueryOrderBy = "";//清除OrderBy
            BaseQuery.AddUnion(resultSelect.BaseQuery, unionType);
            return this;
        }
        string __QueryOrderBy = "";
        /// <summary>
        /// 设置排序
        /// 会重置原排序
        /// </summary>
        /// <typeparam name="TResult2"></typeparam>
        /// <param name="expression"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public LambdaQueryResultSelect<TResult> OrderBy<TResult2>(Expression<Func<TResult, TResult2>> expression, bool desc = true)
        {
            var parameters = expression.Parameters.Select(b => b.Type).ToArray();
            var fields = BaseQuery.GetSelectField(false, expression.Body, false, parameters).fields;
            var orderBy = string.Format(" {0} {1}", fields.First().QueryField, desc ? "desc" : "asc");
            if (!string.IsNullOrEmpty(__QueryOrderBy))
            {
                __QueryOrderBy += ",";
            }
            __QueryOrderBy += orderBy;
            BaseQuery.__QueryOrderBy = __QueryOrderBy;
            return this;
        }
        /// <summary>
        /// 返回自定义类型
        /// </summary>
        /// <typeparam name="TResult2"></typeparam>
        /// <returns></returns>
        public List<TResult2> ToList<TResult2>()
        {
            //todo MongoDB未实现
            var db = DBExtendFactory.CreateDBExtend(BaseQuery.__DbContext);
            return db.QueryResult<TResult2>(BaseQuery);
        }
        /// <summary>
        /// 返回筛选类型
        /// </summary>
        /// <returns></returns>
        public List<TResult> ToList()
        {
            //todo MongoDB未实现
            var db = DBExtendFactory.CreateDBExtend(BaseQuery.__DbContext);
            if (db is DBExtend.MongoDB.MongoDB)
            {
                throw new NotSupportedException("MongoDB暂未实现");
            }
            if (resultSelectorBody is NewExpression)
            {
                var newExpression = resultSelectorBody as NewExpression;
                return db.QueryResult<TResult>(BaseQuery, newExpression);
            }
            throw new CRLException("ToList不支持此表达式 " + resultSelectorBody);
        }
        /// <summary>
        /// 返回动态类型
        /// </summary>
        /// <returns></returns>
        public List<dynamic> ToDynamic()
        { 
            //todo MongoDB未实现
            var db = DBExtendFactory.CreateDBExtend(BaseQuery.__DbContext);
            if (db is DBExtend.MongoDB.MongoDB)
            {
                throw new NotSupportedException("MongoDB暂未实现");
            }
            return db.QueryDynamic(BaseQuery);
        }
    }
}
