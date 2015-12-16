using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CRL.LambdaQuery;
namespace CRL
{
    public sealed partial class DBExtend
    {
        #region delete
        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Delete<TModel>(string where) where TModel : IModel,new()
        {
            CheckTableCreated<TModel>();
            string table = TypeCache.GetTableName(typeof(TModel),dbContext);
            string sql = _DBAdapter.GetDeleteSql(table, where);
            sql = _DBAdapter.SqlFormat(sql);
            int n = dbHelper.Execute(sql);
            ClearParame();
            return n;
        }
        /// <summary>
        /// 按主键删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete<TModel>(int id) where TModel : IModel, new()
        {
            var expression = GetQueryIdExpression<TModel>(id);
            return Delete<TModel>(expression);
        }
        /// <summary>
        /// 指定条件删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : IModel, new()
        {
            LambdaQuery<TModel> query = new LambdaQuery<TModel>(dbContext, false);
            string condition = query.FormatExpression(expression);
            query.FillParames(this);
            return Delete<TModel>(condition);
        }
        #endregion
    }
}
