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
        ///// <summary>
        ///// 动态对象分页
        ///// </summary>
        ///// <typeparam name="TModel"></typeparam>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public override List<dynamic> PageDynamic<TModel>(LambdaQuery<TModel> query)
        //{
        //    int count;
        //    var reader = GetPageReader(query);
        //    var list = reader.GetDataDynamic(out count);
        //    query.MapingTime += reader.runTime;
        //    query.RowCount = count;
        //    return list;
        //}
        ///// <summary>
        ///// 分页
        ///// </summary>
        ///// <typeparam name="TModel"></typeparam>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public override List<TModel> Page<TModel>(LambdaQuery<TModel> query)
        //{
        //    int count;
        //    var reader = GetPageReader(query);
        //    var list = reader.GetDataTResult<TModel>(false, out count, query.GetFieldMapping());
        //    query.MapingTime += reader.runTime;
        //    query.RowCount = count;
        //    return list;
        //}
        ///// <summary>
        ///// 指定对象分页
        ///// </summary>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public override List<TResult> Page<TResult>(LambdaQueryBase query)
        //{
        //    int count;
        //    var reader = GetPageReader(query);
        //    var list = reader.GetDataTResult<TResult>(false, out count, query.GetFieldMapping());
        //    query.MapingTime += reader.runTime;
        //    query.RowCount = count;
        //    return list;
        //}
        #endregion
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
            //dic.Add("pageSize", pageSize.ToString());
            //string sql = string.Format("{0} with(nolock) where {1}", tableName, where);
            string sp = CompileSqlToSp(_DBAdapter.TemplatePage, condition, dic);
            CallBackDataReader reader;
            reader = new CallBackDataReader(__DbHelper.RunDataReader(sp), () =>
            {
                return GetOutParam<int>("count");
            });
            ClearParame();
            query1.ExecuteTime += __DbHelper.ExecuteTime;
            return reader;
        }
        /// <summary>
        /// GROUP和是否编译判断
        /// </summary>
        /// <param name="query1"></param>
        /// <returns></returns>
        internal CallBackDataReader GetPageReader(LambdaQueryBase query1)
        {
            if (query1.__GroupFields.Count > 0)
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
            var condition = query1.GetQueryConditions();

            condition = _DBAdapter.SqlFormat(condition);
            query1.FillParames(this);

            var pageIndex = query1.SkipPage;
            var pageSize = query1.TakeNum;
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            pageSize = pageSize == 0 ? 15 : pageSize;
            string countSql = string.Format("select count(*) from {0}", condition);
            int count = Convert.ToInt32(__DbHelper.ExecScalar(countSql));
            query1.ExecuteTime += __DbHelper.ExecuteTime;
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
            var reader = new CallBackDataReader(__DbHelper.ExecDataReader(sql), () =>
            {
                return count;
            });
            query1.ExecuteTime += __DbHelper.ExecuteTime;
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
            var conditions = query1.GetQueryConditions();
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
            __DbHelper.AddOutParam("count", -1);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("fields", fields);
            dic.Add("rowOver", rowOver);
            //dic.Add("pageSize", pageSize.ToString());
            //dic.Add("sort", sort1);
            string sp = CompileSqlToSp(_DBAdapter.TemplateGroupPage, conditions, dic);
            CallBackDataReader reader;
            reader = new CallBackDataReader(__DbHelper.RunDataReader(sp), () =>
            {
                return GetOutParam<int>("count");
            });
            query1.ExecuteTime += __DbHelper.ExecuteTime;
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
            var condition = query1.GetQueryConditions();
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
            int count = Convert.ToInt32(__DbHelper.ExecScalar(countSql));
            query1.ExecuteTime += __DbHelper.ExecuteTime;
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
            var reader = new CallBackDataReader(__DbHelper.ExecDataReader(sql), () =>
            {
                return count;
            });
            query1.ExecuteTime += __DbHelper.ExecuteTime;
            ClearParame();
            return reader;
        }
        #endregion
    }
}
