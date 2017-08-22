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
        
        #region 按存储过程
        /// <summary>
        /// 按编译
        /// </summary>
        /// <param name="query1"></param>
        /// <returns></returns>
        CallBackDataReader GetSpPageReader(LambdaQueryBase query1)
        {
            //var query1 = query as RelationLambdaQuery<TModel>;
            CheckTableCreated(query1.__MainType);
            //var fields = query.GetQueryFieldString(b => b.Length > 500 || b.PropertyType == typeof(byte[]));
            var fields = query1.GetQueryFieldString();
            var rowOver = query1.__QueryOrderBy;
            if (string.IsNullOrEmpty(rowOver))
            {
                var table = TypeCache.GetTable(query1.__MainType);
                rowOver = string.Format("t1.{0} desc", table.PrimaryKey.MapingName);
            }
            var orderBy = System.Text.RegularExpressions.Regex.Replace(rowOver, @"t\d\.", "t.");
            var sb = new StringBuilder();
            query1.GetQueryConditions(sb);
            var condition = sb.ToString();
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
            //dic.Add("pageSize", pageSize.ToString());
            //string sql = string.Format("{0} with(nolock) where {1}", tableName, where);
            string sp = CompileSqlToSp(_DBAdapter.TemplatePage, condition, dic);
            CallBackDataReader reader;
            var db = GetDBHelper(AccessType.Read);
            reader = new CallBackDataReader(db.RunDataReader(sp), () =>
            {
                return GetOutParam<int>("count");
            },sp);
            ClearParame();
            query1.ExecuteTime += db.ExecuteTime;
            return reader;
        }
        /// <summary>
        /// GROUP和是否编译判断
        /// </summary>
        /// <param name="query1"></param>
        /// <returns></returns>
        internal CallBackDataReader GetPageReader(LambdaQueryBase query1)
        {
            if (query1.__GroupFields != null)
            {
                return GetGroupPageReader(query1);
            }
            if (_DBAdapter.CanCompileSP && query1.__CompileSp)
            {
                return GetSpPageReader(query1);
            }

            CheckTableCreated(query1.__MainType);
            //var fields = query.GetQueryFieldString(b => b.Length > 500 || b.PropertyType == typeof(byte[]));
            var fields = query1.GetQueryFieldString();
            var rowOver = query1.__QueryOrderBy;
            if (string.IsNullOrEmpty(rowOver))
            {
                var table = TypeCache.GetTable(query1.__MainType);
                rowOver = string.Format("t1.{0} desc", table.PrimaryKey.MapingName);
            }
            var orderBy = System.Text.RegularExpressions.Regex.Replace(rowOver, @"t\d\.", "t.");
            var sb = new StringBuilder();
            query1.GetQueryConditions(sb);
            var condition = sb.ToString();

            condition = _DBAdapter.SqlFormat(condition);
            query1.FillParames(this);

            var pageIndex = query1.SkipPage;
            var pageSize = query1.TakeNum;
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            pageSize = pageSize == 0 ? 15 : pageSize;
            string countSql = string.Format("select count(*) from {0}", condition);
            var db = GetDBHelper(AccessType.Read);
            int count = Convert.ToInt32(SqlStopWatch.ExecScalar(db, countSql));
            query1.ExecuteTime += db.ExecuteTime;
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
            var reader = new CallBackDataReader(db.ExecDataReader(sql), () =>
            {
                return count;
            },sql);
            query1.ExecuteTime += db.ExecuteTime;
            ClearParame();
            return reader;
        }
        #endregion

        #region group分页
        /// <summary>
        /// 按编译
        /// </summary>
        /// <param name="query1"></param>
        /// <returns></returns>
        CallBackDataReader GetSpGroupPageReader(LambdaQueryBase query1)
        {
            //var query1 = query as RelationLambdaQuery<TModel>;
            CheckTableCreated(query1.__MainType);
            var sb = new StringBuilder();
            query1.GetQueryConditions(sb);
            var conditions = sb.ToString();
            var fields = query1.GetQueryFieldString();
            if (!conditions.Contains("group"))
            {
                throw new CRLException("缺少group语法");
            }
            var rowOver = query1.__QueryOrderBy;
            if (string.IsNullOrEmpty(rowOver))
            {
                throw new CRLException("Group分页需指定Group排序字段");
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
            var db = GetDBHelper(AccessType.Read);
            db.AddOutParam("count", -1);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("fields", fields);
            dic.Add("rowOver", rowOver);
            //dic.Add("pageSize", pageSize.ToString());
            //dic.Add("sort", sort1);
            string sp = CompileSqlToSp(_DBAdapter.TemplateGroupPage, conditions, dic);
            CallBackDataReader reader;
            reader = new CallBackDataReader(db.RunDataReader(sp), () =>
            {
                return GetOutParam<int>("count");
            },sp);
            query1.ExecuteTime += db.ExecuteTime;
            ClearParame();
            return reader;
        }

        /// <summary>
        /// 按是否能编译
        /// </summary>
        /// <param name="query1"></param>
        /// <returns></returns>
        CallBackDataReader GetGroupPageReader(LambdaQueryBase query1)
        {
            //var query1 = query as RelationLambdaQuery<TModel>;
            if (_DBAdapter.CanCompileSP && query1.__CompileSp)
            {
                return GetSpGroupPageReader(query1);
            }
            CheckTableCreated(query1.__MainType);
            var sb = new StringBuilder();
            query1.GetQueryConditions(sb);
            var condition = sb.ToString();
            var fields = query1.GetQueryFieldString();
            if (!condition.Contains("group"))
            {
                throw new CRLException("缺少group语法");
            }
            var rowOver = query1.__QueryOrderBy;
            if (string.IsNullOrEmpty(rowOver))
            {
                throw new CRLException("Group分页需指定Group排序字段");
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
            var db = GetDBHelper(AccessType.Read);
            int count = Convert.ToInt32(SqlStopWatch.ExecScalar(db, countSql));
            query1.ExecuteTime += db.ExecuteTime;
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
            var reader = new CallBackDataReader(db.ExecDataReader(sql), () =>
            {
                return count;
            },sql);
            query1.ExecuteTime += db.ExecuteTime;
            ClearParame();
            return reader;
        }
        #endregion
    }
}
