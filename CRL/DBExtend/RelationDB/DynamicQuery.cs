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
            //var reader = GetDataReader(sql, types);
            //double runTime;
            //return Dynamic.DynamicObjConvert.DataReaderToDynamic(reader, out runTime);

            var list = SqlStopWatch.ReturnData(() =>
            {
                return GetDataReader(sql, types);

            }, (r) =>
            {
                double runTime;
                return Dynamic.DynamicObjConvert.DataReaderToDynamic(r.reader, out runTime);
            });
            return list;
        }
        /// <summary>
        /// 返回dynamic集合
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public override List<dynamic> RunDynamicList(string sp)
        {
            double runTime;
            //var reader = SqlStopWatch.RunDataReader(__DbHelper,sp);
            //ClearParame();
            //return Dynamic.DynamicObjConvert.DataReaderToDynamic(reader,out runTime);
            var list = SqlStopWatch.ReturnList(() =>
            {
                var db = GetDBHelper(AccessType.Read);
                var reader = db.RunDataReader(sp);
                return Dynamic.DynamicObjConvert.DataReaderToDynamic(reader, out runTime);
            }, sp);
            return list;
        }
        #endregion
        /// <summary>
        /// 返回动态对象
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override List<dynamic> QueryDynamic(LambdaQueryBase query)
        {
            if (query.SkipPage > 0)
            {
                //int count;
                //var reader = GetPageReader(query);
                //var list = reader.GetDataDynamic(out count);
                //query.MapingTime += reader.runTime;
                //query.RowCount = count;
                //return list;

                var list = SqlStopWatch.ReturnData(() =>
                {
                    return GetPageReader(query);

                }, (r) =>
                {
                    int count;
                    var list2 = r.GetDataDynamic(out count);
                    query.MapingTime += r.runTime;
                    query.RowCount = count;
                    return list2;
                });
                return list;
            }
            else
            {
                //var reader = GetQueryDynamicReader(query);
                //double runTime;
                //var list = Dynamic.DynamicObjConvert.DataReaderToDynamic(reader, out runTime);
                //query.MapingTime += runTime;
                //query.RowCount = list.Count;
                //return list;

                var list = SqlStopWatch.ReturnData(() =>
                {
                    return GetQueryDynamicReader(query);
                }, (r) =>
                {
                    double runTime;
                    var list2 = Dynamic.DynamicObjConvert.DataReaderToDynamic(r.reader, out runTime);
                    query.MapingTime += runTime;
                    query.RowCount = list2.Count;
                    return list2;
                });
                return list;
            }
        }
        /// <summary>
        /// 按select返回指定类型
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public override List<TResult> QueryResult<TResult>(LambdaQueryBase query)
        {
            var queryInfo = new LambdaQuery.Mapping.QueryInfo<TResult>(false, query.GetQueryFieldString(), query.GetFieldMapping());
            if (query.SkipPage > 0)
            {
                //var reader = GetPageReader(query);
                //int count;
                //var list = reader.GetDataTResult<TResult>(queryInfo, out count);
                //query.RowCount = count;
                //return list;

                var list = SqlStopWatch.ReturnData(() =>
                {
                    return GetPageReader(query);

                }, (r) =>
                {
                    int count;
                    var list2 = r.GetDataTResult<TResult>(queryInfo, out count);
                    query.RowCount = count;
                    return list2;
                });
                return list;

            }
            else
            {
                //var reader = GetQueryDynamicReader(query);
                //var list = ObjectConvert.DataReaderToSpecifiedList<TResult>(reader, queryInfo);
                //query.RowCount = list.Count;
                //return list;
                var list = SqlStopWatch.ReturnData(() =>
                {
                    return GetQueryDynamicReader(query);

                }, (r) =>
                {
                    var list2 = ObjectConvert.DataReaderToSpecifiedList<TResult>(r.reader, queryInfo);
                    query.RowCount = list2.Count;
                    return list2;
                });
                return list;
            }
        }

        /// <summary>
        /// 按匿名对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="newExpression"></param>
        /// <returns></returns>
        public override List<TResult> QueryResult<TResult>(LambdaQueryBase query, NewExpression newExpression)
        {
            List<TResult> list;
            var queryInfo = new LambdaQuery.Mapping.QueryInfo<TResult>(true, query.GetQueryFieldString(), null, newExpression.Constructor);
            if (query.SkipPage > 0)
            {
                //var reader = GetPageReader(query);
                //int count;
                //list = reader.GetDataTResult<TResult>(queryInfo, out count);
                //query.RowCount = count;
                list = SqlStopWatch.ReturnData(() =>
                {
                    return GetPageReader(query);
                }, (r) =>
                {
                    int count;
                    var list2 = r.GetDataTResult<TResult>(queryInfo, out count);
                    query.RowCount = count;
                    return list2;
                });
            }
            else
            {
                //var reader = GetQueryDynamicReader(query);
                //list = ObjectConvert.DataReaderToSpecifiedList<TResult>(reader, queryInfo);
                list = SqlStopWatch.ReturnData(() =>
                {
                    return GetQueryDynamicReader(query);

                }, (r) =>
                {
                    var list2 = ObjectConvert.DataReaderToSpecifiedList<TResult>(r.reader, queryInfo);
                    return list2;
                });
            }
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
            query.TakeNum = 1;
            //using (var reader = GetQueryDynamicReader(query))
            //{
            //    var a = reader.Read();
            //    if (!a)
            //    {
            //        return null;
            //    }
            //    var result = reader[0];
            //    return result;
            //}
            var list = SqlStopWatch.ReturnData(() =>
            {
                return GetQueryDynamicReader(query);

            }, (r) =>
            {
                var reader = r.reader;
                var a = reader.Read();
                if (!a)
                {
                    return null;
                }
                var result = reader[0];
                reader.Close();
                return result;
            });
            return list;

        }
        /// <summary>
        /// 返回动态对象的查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        internal CallBackDataReader GetQueryDynamicReader(LambdaQueryBase query)
        {
            CheckTableCreated(query.__MainType);
            var sql = "";
            query.FillParames(this);
            sql = query.GetQuery();
            sql = _DBAdapter.SqlFormat(sql);
            System.Data.Common.DbDataReader reader;
            var compileSp = query.__CompileSp;
            var db = GetDBHelper(AccessType.Read);
            if (!compileSp)
            {
                if (query.TakeNum > 0)
                {
                    db.AutoFormatWithNolock = false;
                }
                reader = db.ExecDataReader(sql);
            }
            else//生成储过程
            {
                string sp = CompileSqlToSp(_DBAdapter.TemplateSp, sql);
                reader = db.RunDataReader(sp);
            }
            query.ExecuteTime = db.ExecuteTime;
            ClearParame();
            return new CallBackDataReader(reader, null, sql);
        }
    }
}
