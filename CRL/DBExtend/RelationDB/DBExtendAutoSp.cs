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
using System.Text;
using CRL.LambdaQuery;
namespace CRL.DBExtend.RelationDB
{
    public sealed partial class DBExtend
    { 
        #region sql to sp

        static Dictionary<string, int> spCahe = new Dictionary<string, int>();
        /// <summary>
        /// 将SQL语句编译成储存过程
        /// </summary>
        /// <param name="template">模版</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parames">模版替换参数</param>
        /// <returns></returns>
        string CompileSqlToSp(string template, string sql, Dictionary<string, string> parames = null)
        {
            if (!_DBAdapter.CanCompileSP)
            {
                throw new CRLException("当前数据库不支持动态编译");
            }
            sql = _DBAdapter.SqlFormat(sql);
            lock (lockObj)
            {
                if (spCahe.Count == 0)//初始已编译过的存储过程
                {
                    var db = GetBackgroundDBExtend();
                    //BackupParams();
                    spCahe = db.ExecDictionary<string, int>(_DBAdapter.GetAllSPSql());
                    //RecoveryParams();
                }
            }
            string fields = "";
            if (parames != null)
            {
                if (parames.ContainsKey("fields"))
                {
                    fields = parames["fields"];
                }
                if (parames.ContainsKey("sort"))
                {
                    fields += "_" + parames["sort"];
                }
                if (parames.ContainsKey("rowOver"))
                {
                    fields += "_" + parames["rowOver"];
                }
            }
            string sp = CoreHelper.StringHelper.EncryptMD5(fields + "_" + sql.Trim());
            sp = "ZautoSp_" + sp.Substring(8, 16);
            if (!spCahe.ContainsKey(sp))
            {
                sql = __DbHelper.FormatWithNolock(sql);
                var db = GetBackgroundDBExtend();
                string spScript = Base.SqlToProcedure(template, dbContext, sql, sp, parames);
                try
                {
                    //BackupParams();
                    db.Execute(spScript);
                    //RecoveryParams();
                    lock (lockObj)
                    {
                        if (!spCahe.ContainsKey(sp))
                        {
                            spCahe.Add(sp, 0);
                        }
                    }
                    string log = string.Format("创建存储过程:{0}\r\n{1}", sp, spScript);
                    CoreHelper.EventLog.Log(log, "sqlToSp", false);
                }
                catch (Exception ero)
                {
                    //RecoveryParams();
                    throw new CRLException("动态创建存储过程时发生错误:" + ero.Message);
                }
            }
            return sp;
        }

        ///// <summary>
        ///// 将查询自动转化为存储过程执行
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="sql"></param>
        ///// <param name="types"></param>
        ///// <returns></returns>
        //public override List<T> AutoSpQuery<T>(string sql, params Type[] types) 
        //{
        //    var reader = AutoSpQuery(sql, types);
        //    double runTime;
        //    return ObjectConvert.DataReaderToList<T>(reader,out runTime);
        //}
        //System.Data.Common.DbDataReader AutoSpQuery(string sql, params Type[] types) 
        //{
        //    sql = AutoFormat(sql, types);
        //    sql = _DBAdapter.SqlFormat(sql);
        //    string sp = CompileSqlToSp(_DBAdapter.TemplateSp, sql);
        //    System.Data.Common.DbDataReader reader;
        //    reader = dbHelper.RunDataReader(sp);
        //    ClearParame();
        //    return reader;
        //}
        ///// <summary>
        ///// 将查询自动转化为存储过程执行
        ///// </summary>
        ///// <typeparam name="TKey"></typeparam>
        ///// <typeparam name="TValue"></typeparam>
        ///// <param name="sql"></param>
        ///// <returns></returns>
        //public override Dictionary<TKey, TValue> AutoSpQuery<TKey, TValue>(string sql, params Type[] types)
        //{
        //    sql = AutoFormat(sql, types);
        //    sql = _DBAdapter.SqlFormat(sql);
        //    string sp = CompileSqlToSp(_DBAdapter.TemplateSp, sql);
        //    System.Data.Common.DbDataReader reader = null;
        //    reader = dbHelper.RunDataReader(sp);
        //    ClearParame();
        //    return ObjectConvert.DataReadToDictionary<TKey, TValue>(reader);
        //}
        ///// <summary>
        ///// 将更新自动转化为存储过程执行
        ///// </summary>
        ///// <param name="sql"></param>
        ///// <param name="types"></param>
        ///// <returns></returns>
        //public override int AutoSpUpdate(string sql, params Type[] types)
        //{
        //    int n;
        //    sql = AutoFormat(sql, types);
        //    sql = _DBAdapter.SqlFormat(sql);
        //    string sp = CompileSqlToSp(_DBAdapter.TemplateSp, sql);
        //    n = dbHelper.Run(sp);
        //    ClearParame();
        //    return n;
        //}
        ///// <summary>
        ///// 返回首行首列
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="sql"></param>
        ///// <param name="types"></param>
        ///// <returns></returns>
        //public override T AutoExecuteScalar<T>(string sql,params Type[] types)
        //{
        //    object obj;
        //    sql = _DBAdapter.SqlFormat(sql);
        //    sql = AutoFormat(sql, types);
        //    sql = _DBAdapter.SqlFormat(sql);
        //    string sp = CompileSqlToSp(_DBAdapter.TemplateSp, sql);
        //    obj = RunScalar(sp);
        //    ClearParame();
        //    return ObjectConvert.ConvertObject<T>(obj);
        //}
        #endregion
    }
}
