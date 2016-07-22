/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CRL.LambdaQuery;
namespace CRL.DBExtend.RelationDB
{
    //返回动态类型
    public sealed partial class DBExtend
    {
        #region sql语句
        /// <summary>
        /// 返回dynamic集合
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public override List<dynamic> ExecDynamicList(string sql, params Type[] types)
        {
            var reader = GetDataReader(sql, types);
            double runTime;
            return Dynamic.DynamicObjConvert.DataReaderToDynamic(reader, out runTime);
        }
        /// <summary>
        /// 返回dynamic集合
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public override List<dynamic> RunDynamicList(string sp)
        {
            double runTime;
            var reader = dbHelper.RunDataReader(sp);
            ClearParame();
            return Dynamic.DynamicObjConvert.DataReaderToDynamic(reader,out runTime);
        }
        #endregion
        /// <summary>
        /// 返回动态对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public override List<dynamic> QueryDynamic<T>(LambdaQuery<T> query)
        {
            var reader = GetQueryDynamicReader(query);
            double runTime;
            var list = Dynamic.DynamicObjConvert.DataReaderToDynamic(reader, out runTime);
            query.MapingTime += runTime;
            query.RowCount = list.Count;
            return list;
        }
        /// <summary>
        /// 按select返回指定类型
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public override List<TResult> QueryDynamic<TModel, TResult>(LambdaQuery<TModel> query)
        {
            var reader = GetQueryDynamicReader(query);
            double runTime;
            var list = ObjectConvert.DataReaderToList<TResult>(reader, out runTime, false);
            query.MapingTime += runTime;
            query.RowCount = list.Count;
            return list;
        }
        /// <summary>
        /// 返回首列结果
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public override dynamic QueryScalar<TModel>(LambdaQuery<TModel> query)
        {
            query.Top(1);
            var reader = GetQueryDynamicReader(query);
            var a = reader.Read();
            if (!a)
            {
                return null;
            }
            var result = reader[0];
            reader.Close();
            return result;
        }
        /// <summary>
        /// 返回动态对象的查询
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        internal System.Data.Common.DbDataReader GetQueryDynamicReader<TModel>(LambdaQuery<TModel> query) where TModel : CRL.IModel, new()
        {
            CheckTableCreated<TModel>();
            string sql = "";
            query.FillParames(this);
            sql = query.GetQuery();
            sql = _DBAdapter.SqlFormat(sql);
            System.Data.Common.DbDataReader reader;
            var compileSp = query.__CompileSp;
            if (!compileSp)
            {
                if (query.TakeNum > 0)
                {
                    dbHelper.AutoFormatWithNolock = false;
                }
                reader = dbHelper.ExecDataReader(sql);
            }
            else//生成储过程
            {
                string sp = CompileSqlToSp(_DBAdapter.TemplateSp, sql);
                reader = dbHelper.RunDataReader(sp);
            }
            query.ExecuteTime = dbHelper.ExecuteTime;
            ClearParame();
            return reader;

        }

        /// <summary>
        /// 按筛选返回匿名类型
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public override List<TResult> QueryDynamic<TModel, TResult>(LambdaQuery<TModel> query, Expression<Func<TModel, TResult>> resultSelector)
        {
            //todo 由于不能自动识别TResult,只能按当前类型筛选
            CheckTableCreated<TModel>();
            query.Select(resultSelector.Body);
            var reader = GetQueryDynamicReader(query);
            double runTime;
            var list = Dynamic.DynamicObjConvert.DataReaderToDynamic(reader, resultSelector, out runTime);
            query.MapingTime += runTime;
            query.RowCount = list.Count;
            return list;
        }
    }
}
