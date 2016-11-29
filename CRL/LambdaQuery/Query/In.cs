using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery
{
    public abstract partial class LambdaQuery<T> : LambdaQueryBase where T : IModel, new()
    {
        #region 按完整子查询
        /// <summary>
        /// 按查询exists
        /// 等效为exixts(select field from table2)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public LambdaQuery<T> Exists<TResult>(LambdaQueryResultSelect<TResult> query)
        {
            return InnerSelect(null, query, "exists");
        }

        /// <summary>
        /// 按查询not exists
        /// 等效为 not exixts(select field from table2)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public LambdaQuery<T> NotExists<TResult>(LambdaQueryResultSelect<TResult> query)
        {
            return InnerSelect(null, query, "not exists");
        }

        /// <summary>
        /// 按查询in
        /// 等效为table.field in(select field from table2)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="outField"></param>
        /// <returns></returns>
        public LambdaQuery<T> In<TResult>(LambdaQueryResultSelect<TResult> query, Expression<Func<T, TResult>> outField)
        {
            return InnerSelect(outField, query, "in");
        }

        /// <summary>
        /// 按查询not in
        /// 等效为table.field not in(select field from table2)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="outField"></param>
        /// <returns></returns>
        public LambdaQuery<T> NotIn<TResult>(LambdaQueryResultSelect<TResult> query, Expression<Func<T, TResult>> outField)
        {
            return InnerSelect(outField, query, "not in");
        }

        /// <summary>
        /// 按=
        /// 等效为table.field =(select field from table2)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="outField"></param>
        /// <returns></returns>
        public LambdaQuery<T> Equal<TResult>(LambdaQueryResultSelect<TResult> query, Expression<Func<T, TResult>> outField)
        {
            return InnerSelect(outField, query, "=");
        }

        /// <summary>
        /// 按!=
        /// 等效为table.field !=(select field from table2)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="outField"></param>
        /// <returns></returns>
        public LambdaQuery<T> NotEqual<TResult>(LambdaQueryResultSelect<TResult> query, Expression<Func<T, TResult>> outField)
        {
            return InnerSelect(outField, query, "!=");
        }

        LambdaQuery<T> InnerSelect<TResult>(Expression<Func<T, TResult>> outField, LambdaQueryResultSelect<TResult> query, string type, string innerJoinSql = "")
        {
            if (!query.BaseQuery.__FromDbContext)
            {
                throw new CRLException("关联需要由LambdaQuery.CreateQuery创建");
            }
            var baseQuery = query.BaseQuery;
            foreach (var kv in baseQuery.QueryParames)
            {
                QueryParames[kv.Key] = kv.Value;
            }
            var query2 = baseQuery.GetQuery();
            return InnerSelect(outField, query2, type);
        }
        LambdaQuery<T> InnerSelect<TResult>(Expression<Func<T, TResult>> outField, string query, string type)
        {
            MemberExpression m1 = null;
            //object 会生成UnaryExpression表达式 Convert(b=>b.UserId)
            if (outField != null)//兼容exists 可能为空
            {
                if (outField.Body is UnaryExpression)
                {
                    m1 = (outField.Body as UnaryExpression).Operand as MemberExpression;
                }
                else
                {
                    m1 = outField.Body as MemberExpression;
                }
            }
            string field1 = "";
            if (outField != null)
            {
                var f = TypeCache.GetProperties(typeof(T), true)[m1.Member.Name];
                field1 = string.Format("{0}{1}", GetPrefix(typeof(T)), __DBAdapter.KeyWordFormat(f.MapingName));
            }
            string condition = "";
            condition = string.Format("{0} {1}({2})", field1, type, query);
            if (Condition.Length > 0)
            {
                condition = " and " + condition;
            }
            Condition.Append(condition);
            return this;
        }
        #endregion
        #region 表达式关联
        LambdaQuery<T> InnerSelect2<TInner>(Expression<Func<T, object>> outField, Expression<Func<TInner, object>> innerField,
    Expression<Func<T, TInner, bool>> expression, string type) where TInner : IModel, new()
        {
            MemberExpression m2 = null;
            if (innerField.Body is UnaryExpression)
            {
                m2 = (innerField.Body as UnaryExpression).Operand as MemberExpression;
            }
            else
            {
                m2 = innerField.Body as MemberExpression;
            }
            var f = TypeCache.GetProperties(typeof(TInner), true)[m2.Member.Name];
            var prefix = GetPrefix(typeof(TInner));
            var tableName = TypeCache.GetTableName(typeof(TInner), __DbContext);
            tableName += " "+prefix.Substring(0, prefix.Length - 1);
            var field2 = string.Format("{0}{1}", prefix, __DBAdapter.KeyWordFormat(f.MapingName));
            string condition = FormatJoinExpression(expression.Body);
            var query = string.Format("select {0} from {1} where {2}", field2, tableName + __DBAdapter.GetWithNolockFormat(__WithNoLock), condition);
            return InnerSelect(outField, query, type); 
        }

        /// <summary>
        /// 按查询exists
        /// 等效为exixts(select field from table2)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="innerField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> Exists<TInner>(Expression<Func<TInner, object>> innerField,
Expression<Func<T, TInner, bool>> expression) where TInner : IModel, new()
        {
            return InnerSelect2<TInner>(null, innerField, expression, "exists");
        }
        /// <summary>
        /// 按查询not exists
        /// 等效为not exixts(select field from table2)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="innerField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> NotExists<TInner>(Expression<Func<TInner, object>> innerField,
Expression<Func<T, TInner, bool>> expression) where TInner : IModel, new()
        {
            return InnerSelect2<TInner>(null, innerField, expression, "not exists");
        }

        /// <summary>
        /// 按查询in
        /// 等效为table.field in(select field from table2)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="outField"></param>
        /// <param name="innerField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> In<TInner>(Expression<Func<T, object>> outField, Expression<Func<TInner, object>> innerField,
    Expression<Func<T, TInner, bool>> expression) where TInner : IModel, new()
        {
            return InnerSelect2<TInner>(outField, innerField, expression, "in");
        }
        /// <summary>
        /// 按查询not in
        /// 等效为table.field not in(select field from table2)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="outField"></param>
        /// <param name="innerField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> NotIn<TInner>(Expression<Func<T, object>> outField, Expression<Func<TInner, object>> innerField,
Expression<Func<T, TInner, bool>> expression) where TInner : IModel, new()
        {
            return InnerSelect2<TInner>(outField, innerField, expression, "not in");
        }
        /// <summary>
        /// 按=
        /// 等效为table.field =(select field from table2)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="outField"></param>
        /// <param name="innerField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> Equal<TInner>(Expression<Func<T, object>> outField, Expression<Func<TInner, object>> innerField,
Expression<Func<T, TInner, bool>> expression) where TInner : IModel, new()
        {
            return InnerSelect2<TInner>(outField, innerField, expression, "=");
        }
        /// <summary>
        /// 按!=
        /// 等效为table.field !=(select field from table2)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="outField"></param>
        /// <param name="innerField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> NotEqual<TInner>(Expression<Func<T, object>> outField, Expression<Func<TInner, object>> innerField,
Expression<Func<T, TInner, bool>> expression) where TInner : IModel, new()
        {
            return InnerSelect2<TInner>(outField, innerField, expression, "!=");
        }

        #endregion
    }
}
