/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CRL.LambdaQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CRL.DBExtend.RelationDB
{
    public sealed partial class DBExtend
    {
        #region 分页
        /// <summary>
        /// 动态对象分页
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public override List<dynamic> Page<TModel>(LambdaQuery<TModel> query)
        {
            int count;
            var reader = GetPageReader(query);
            var list = reader.GetDataDynamic(out count);
            query.MapingTime += reader.runTime;
            query.RowCount = count;
            return list;
        }    
        /// <summary>
        /// 指定对象分页
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public override List<TResult> Page<TModel, TResult>(LambdaQuery<TModel> query)
        {
            int count;
            var reader = GetPageReader(query);
            var list = reader.GetData<TResult>(out count);
            query.MapingTime += reader.runTime;
            query.RowCount = count;
            return list;
        }
        /// <summary>
        /// 按编译
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        CallBackDataReader GetSpPageReader<TModel>(LambdaQuery<TModel> query) where TModel : IModel, new()
        {
            var query1 = query as RelationLambdaQuery<TModel>;
            CheckTableCreated<TModel>();
            //var fields = query.GetQueryFieldString(b => b.Length > 500 || b.PropertyType == typeof(byte[]));
            var fields = query1.GetQueryFieldString();
            var rowOver = query1.__QueryOrderBy;
            if (string.IsNullOrEmpty(rowOver))
            {
                var table = TypeCache.GetTable(typeof(TModel));
                rowOver = string.Format("t1.{0} desc", table.PrimaryKey.MappingName);
            }
            var orderBy = System.Text.RegularExpressions.Regex.Replace(rowOver, @"t\d\.", "t.");
            var condition = query1.GetQueryConditions();
            condition = _DBAdapter.SqlFormat(condition);
            query1.FillParames(this);
            var pageIndex = query1.SkipPage;
            var pageSize = query1.TakeNum;
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            pageSize = pageSize == 0 ? 15 : pageSize;
            AddParam("pageIndex", pageIndex);
            AddParam("pageSize", pageSize);
            AddOutParam("count", -1);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("fields", fields);
            dic.Add("sort", orderBy);
            dic.Add("rowOver", rowOver);
            //string sql = string.Format("{0} with(nolock) where {1}", tableName, where);
            string sp = CompileSqlToSp(_DBAdapter.TemplatePage, condition, dic);
            CallBackDataReader reader;
            reader = new CallBackDataReader(dbHelper.RunDataReader(sp), () =>
            {
                return GetOutParam<int>("count");
            });
            ClearParame();
            query1.ExecuteTime += dbHelper.ExecuteTime;
            return reader;
        }
        /// <summary>
        /// GROUP和是否编译判断
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        internal CallBackDataReader GetPageReader<TModel>(LambdaQuery<TModel> query) where TModel : IModel, new()
        {
            var query1 = query as RelationLambdaQuery<TModel>;
            if (query1.__GroupFields.Count > 0)
            {
                return GetGroupPageReader(query1);
            }
            if (_DBAdapter.CanCompileSP && query1.__CompileSp)
            {
                return GetSpPageReader(query1);
            }

            CheckTableCreated<TModel>();
            //var fields = query.GetQueryFieldString(b => b.Length > 500 || b.PropertyType == typeof(byte[]));
            var fields = query1.GetQueryFieldString();
            var rowOver = query1.__QueryOrderBy;
            if (string.IsNullOrEmpty(rowOver))
            {
                var table = TypeCache.GetTable(typeof(TModel));
                rowOver = string.Format("t1.{0} desc", table.PrimaryKey.MappingName);
            }
            var orderBy = System.Text.RegularExpressions.Regex.Replace(rowOver, @"t\d\.", "t.");
            var condition = query1.GetQueryConditions();

            condition = _DBAdapter.SqlFormat(condition);
            query1.FillParames(this);

            var pageIndex = query1.SkipPage;
            var pageSize = query1.TakeNum;
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            pageSize = pageSize == 0 ? 15 : pageSize;
            string countSql = string.Format("select count(*) from {0}", condition);
            int count = Convert.ToInt32(dbHelper.ExecScalar(countSql));
            query1.ExecuteTime += dbHelper.ExecuteTime;
            query1.RowCount = count;
            //if (count == 0)
            //{
            //    return null;
            //}
            int pageCount = (count + pageSize - 1) / pageSize;
            if (pageIndex > pageCount)
                pageIndex = pageCount;

            var start = pageSize * (pageIndex - 1) + 1;
            var end = start + pageSize - 1;
            string sql = _DBAdapter.PageSqlFormat(fields, rowOver, condition, start, end, orderBy);
            var reader = new CallBackDataReader(dbHelper.ExecDataReader(sql), () =>
            {
                return count;
            });
            query1.ExecuteTime += dbHelper.ExecuteTime;
            ClearParame();
            return reader;
        }
        #endregion

        #region group分页
        /// <summary>
        /// 按编译
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        CallBackDataReader GetSpGroupPageReader<TModel>(LambdaQuery<TModel> query) where TModel : IModel, new()
        {
            var query1 = query as RelationLambdaQuery<TModel>;
            CheckTableCreated<TModel>();
            var conditions = query1.GetQueryConditions();
            var fields = query1.GetQueryFieldString();
            if (!conditions.Contains("group"))
            {
                throw new Exception("缺少group语法");
            }
            var rowOver = query1.__QueryOrderBy;
            if (string.IsNullOrEmpty(rowOver))
            {
                throw new Exception("Group分页需指定Group排序字段");
                //var table = TypeCache.GetTable(typeof(T));
                //rowOver = string.Format("t1.{0} desc", table.PrimaryKey.Name);
            }
            var sort1 = System.Text.RegularExpressions.Regex.Replace(rowOver, @"t\d\.", "");
            conditions = _DBAdapter.SqlFormat(conditions);

            query1.FillParames(this);
            var pageIndex = query1.SkipPage;
            var pageSize = query1.TakeNum;
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            pageSize = pageSize == 0 ? 15 : pageSize;
            AddParam("pageIndex", pageIndex);
            AddParam("pageSize", pageSize);
            dbHelper.AddOutParam("count", -1);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("fields", fields);
            dic.Add("rowOver", rowOver);
            //dic.Add("sort", sort1);
            string sp = CompileSqlToSp(_DBAdapter.TemplateGroupPage, conditions, dic);
            CallBackDataReader reader;
            reader = new CallBackDataReader(dbHelper.RunDataReader(sp), () =>
            {
                return GetOutParam<int>("count");
            });
            query1.ExecuteTime += dbHelper.ExecuteTime;
            ClearParame();
            return reader;
        }

        /// <summary>
        /// 按是否能编译
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        CallBackDataReader GetGroupPageReader<TModel>(LambdaQuery<TModel> query) where TModel : IModel, new()
        {
            var query1 = query as RelationLambdaQuery<TModel>;
            if (_DBAdapter.CanCompileSP && query1.__CompileSp)
            {
                return GetSpGroupPageReader(query1);
            }
            CheckTableCreated<TModel>();
            var condition = query1.GetQueryConditions();
            var fields = query1.GetQueryFieldString();
            if (!condition.Contains("group"))
            {
                throw new Exception("缺少group语法");
            }
            var rowOver = query1.__QueryOrderBy;
            if (string.IsNullOrEmpty(rowOver))
            {
                throw new Exception("Group分页需指定Group排序字段");
                //var table = TypeCache.GetTable(typeof(T));
                //rowOver = string.Format("t1.{0} desc", table.PrimaryKey.Name);
            }
            var sort1 = System.Text.RegularExpressions.Regex.Replace(rowOver, @"t\d\.", "");
            condition = _DBAdapter.SqlFormat(condition);

            query1.FillParames(this);
            var pageIndex = query1.SkipPage;
            var pageSize = query1.TakeNum;
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            pageSize = pageSize == 0 ? 15 : pageSize;

            string countSql = string.Format("select count(*)  from (select count(*) as a from {0}) t", condition);
            int count = Convert.ToInt32(dbHelper.ExecScalar(countSql));
            query1.ExecuteTime += dbHelper.ExecuteTime;
            query1.RowCount = count;
            //if (count == 0)
            //{
            //    return null;
            //}
            int pageCount = (count + pageSize - 1) / pageSize;
            if (pageIndex > pageCount)
                pageIndex = pageCount;

            var start = pageSize * (pageIndex - 1) + 1;
            var end = start + pageSize - 1;
            string sql = _DBAdapter.PageSqlFormat(fields, rowOver, condition, start, end, "");
            //System.Data.Common.DbDataReader reader;
            //reader = dbHelper.ExecDataReader(sql);
            var reader = new CallBackDataReader(dbHelper.ExecDataReader(sql), () =>
            {
                return count;
            });
            query1.ExecuteTime += dbHelper.ExecuteTime;
            ClearParame();
            return reader;
        }
        #endregion
    }
}
