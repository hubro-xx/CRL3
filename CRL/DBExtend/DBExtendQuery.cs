using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using CRL.LambdaQuery;
namespace CRL
{
    public sealed partial class DBExtend
    {
        #region query item
        /// <summary>
        /// 按ID查询
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public TItem QueryItem<TItem>(int id) where TItem : IModel, new()
        {
            var expression = GetQueryIdExpression<TItem>(id);
            return QueryItem<TItem>(expression);
        }
        /// <summary>
        /// 查询返回单个结果
        /// 如果只查询ID,调用QueryItem(id)
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="expression"></param>
        /// <param name="idDest">是否按主键倒序</param>
        /// <param name="compileSp">是否编译成存储过程</param>
        /// <returns></returns>
        public TItem QueryItem<TItem>(Expression<Func<TItem, bool>> expression, bool idDest = true, bool compileSp = false) where TItem : IModel, new()
        {
            LambdaQuery<TItem> query = new LambdaQuery<TItem>(dbContext);
            query.Top(1);
            query.Where(expression);
            query.CompileToSp(compileSp);
            query.OrderByPrimaryKey(idDest);
            List<TItem> list = QueryList<TItem>(query);
            if (list.Count == 0)
                return null;
            return list[0];
        }
        #endregion

        #region query list
        /// <summary>
        /// 使用lamada设置条件查询
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="expression"></param>
        /// <param name="compileSp">是否编译成储过程</param>
        /// <returns></returns>
        public List<TItem> QueryList<TItem>(Expression<Func<TItem, bool>> expression =null, bool compileSp = false) where TItem : IModel, new()
        {
            LambdaQuery<TItem> query = new LambdaQuery<TItem>(dbContext);
            query.Where(expression);
            query.CompileToSp(compileSp);
            return QueryList<TItem>(query);
        }
        

        /// <summary>
        /// 使用完整的LamadaQuery查询
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<TItem> QueryList<TItem>(LambdaQuery<TItem> query) where TItem : IModel, new()
        {
            string key;
            return QueryList<TItem>(query, out key);
        }
        /// <summary>
        /// 使用完整的LamadaQuery查询
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="query"></param>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns></returns>
        public List<TItem> QueryList<TItem>(LambdaQuery<TItem> query, out string cacheKey) where TItem : IModel, new()
        {
            CheckTableCreated<TItem>();
            if (query.__PageSize > 0)//按分页
            {
                cacheKey = "";
                return Page<TItem, TItem>(query);
            }
            string sql = "";
            cacheKey = "";
            query.FillParames(this);
            sql = query.GetQuery();
            sql = _DBAdapter.SqlFormat(sql);
            System.Data.Common.DbDataReader reader;
            var cacheTime = query.__ExpireMinute;
            var compileSp = query.__CompileSp;
            List<TItem> list;
            double runTime;
            if (cacheTime <= 0)
            {
                if (!compileSp)
                {
                    if (query.__QueryTop > 0)
                    {
                        dbHelper.AutoFormatWithNolock = false;
                    }
                    reader = dbHelper.ExecDataReader(sql);
                }
                else//生成储过程
                {
                    string sp = CompileSqlToSp(_DBAdapter.TemplateSp, sql);
                    reader = dbHelper.RunDataReader(sql);
                }
                query.ExecuteTime += dbHelper.ExecuteTime;
                list = ObjectConvert.DataReaderToList<TItem>(reader, out runTime, true);
                query.MapingTime += runTime;
            }
            else
            {
                list = MemoryDataCache.CacheService.GetCacheList<TItem>(sql, cacheTime, dbHelper, out cacheKey).Values.ToList();
            }
            ClearParame();
            query.RowCount = list.Count;
            SetOriginClone(list);
            return list;
        }
       
        
        #endregion

        #region count
        /// <summary>
        /// count
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="expression"></param>
        /// <param name="compileSp"></param>
        /// <returns></returns>
        public int Count<TType>(Expression<Func<TType, bool>> expression, bool compileSp = false) where TType : IModel, new()
        {
            return GetFunction<int, TType>(expression, b=>0, FunctionType.COUNT, compileSp);
        }

        #endregion
        /// <summary>
        /// 最小值
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="field"></param>
        /// <param name="compileSp"></param>
        /// <returns></returns>
        public TType Min<TType, TModel>(Expression<Func<TModel, bool>> expression, Expression<Func<TModel, TType>> field, bool compileSp = false) where TModel : IModel, new()
        {
            return GetFunction<TType, TModel>(expression, field, FunctionType.MIN, compileSp);
        }
        #region Max
        /// <summary>
        /// 最大值
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="field"></param>
        /// <param name="compileSp"></param>
        /// <returns></returns>
        public TType Max<TType, TModel>(Expression<Func<TModel, bool>> expression, Expression<Func<TModel, TType>> field, bool compileSp = false) where TModel : IModel, new()
        {
            return GetFunction<TType, TModel>(expression, field, FunctionType.MAX, compileSp);
        }

        #endregion

        #region SUM
        /// <summary>
        /// sum
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="field"></param>
        /// <param name="compileSp"></param>
        /// <returns></returns>
        public TType Sum<TType, TModel>(Expression<Func<TModel, bool>> expression, Expression<Func<TModel, TType>> field, bool compileSp = false) where TModel : IModel, new()
        {
            return GetFunction<TType, TModel>(expression, field, FunctionType.SUM, compileSp);
        }
        //public TType Sum<TType, TModel>(Expression<Func<TModel, bool>> expression, string field, bool compileSp = false) where TModel : IModel, new()
        //{
        //    return GetFunction<TType, TModel>(expression, field, FunctionType.SUM, compileSp);
        //}
        #endregion

        TType GetFunction<TType, TModel>(Expression<Func<TModel, bool>> expression, Expression<Func<TModel, TType>> selectField, FunctionType functionType, bool compileSp = false) where TModel : IModel, new()
        {
            LambdaQuery<TModel> query = new LambdaQuery<TModel>(dbContext, true);
            query.Select(selectField.Body);
            query.__FieldFunctionFormat = string.Format("{0}({1}) as Total", functionType, "{0}");
            query.Where(expression);
            var result = QueryScalar(query);
            if (result == null)
            {
                return default(TType);
            }
            return (TType)result;
            //string condition = query.FormatExpression(expression);
            //query.FillParames(this);
            //string field = query.GetQueryFieldString();
            
            //CheckTableCreated<TModel>();
            //string tableName = TypeCache.GetTableName(typeof(TModel), dbContext);
            //tableName = _DBAdapter.KeyWordFormat(tableName);
            //string sql = "select " + functionType + "(" + field + ") from " + tableName + ' ' + _DBAdapter.GetWithNolockFormat() + " where " + condition;
            //if (compileSp)
            //{
            //    return AutoExecuteScalar<TType>(sql);
            //}
            //return ExecScalar<TType>(sql);
        }
        //TType GetFunction<TType, TModel>(Expression<Func<TModel, bool>> expression, string field, FunctionType functionType, bool compileSp = false) where TModel : IModel, new()
        //{
        //    LambdaQuery<TModel> query = new LambdaQuery<TModel>(dbContext, false);
        //    string condition = query.FormatExpression(expression);
        //    query.FillParames(this);
            
        //    CheckTableCreated<TModel>();
        //    string tableName = TypeCache.GetTableName(typeof(TModel), dbContext);
        //    tableName = _DBAdapter.KeyWordFormat(tableName);
        //    string sql = "select " + functionType + "(" + field + ") from " + tableName + ' ' + _DBAdapter.GetWithNolockFormat() + " where " + condition;
        //    if (compileSp)
        //    {
        //        return AutoExecuteScalar<TType>(sql);
        //    }
        //    return ExecScalar<TType>(sql);
        //}

        /// <summary>
        /// 创建副本
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="list"></param>
        void SetOriginClone<TItem>(List<TItem> list) where TItem : IModel, new()
        {
            if (SettingConfig.UsePropertyChange)
            {
                return;
            }
            foreach (var item in list)
            {
                TItem clone = item.Clone() as TItem;
                clone.OriginClone = null;
                item.OriginClone = clone;
            }
        }
    }
}
