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
using System.Text.RegularExpressions;
using CRL.LambdaQuery;
namespace CRL.DBExtend.RelationDB
{
    public sealed partial class DBExtend
    {
        

        #region query list
       
        /// <summary>
        /// 使用完整的LamadaQuery查询
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns></returns>
        public override List<TModel> QueryList<TModel>(LambdaQuery<TModel> query, out string cacheKey)
        {
            CheckTableCreated<TModel>();
            if (query.SkipPage > 0)//按分页
            {
                cacheKey = "";
                return Page<TModel, TModel>(query);
            }
            string sql = "";
            cacheKey = "";
            query.FillParames(this);
            sql = query.GetQuery();
            sql = _DBAdapter.SqlFormat(sql);
            System.Data.Common.DbDataReader reader;
            var cacheTime = query.__ExpireMinute;
            var compileSp = query.__CompileSp;
            List<TModel> list;
            double runTime;
            if (cacheTime <= 0)
            {
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
                query.ExecuteTime += dbHelper.ExecuteTime;
                list = ObjectConvert.DataReaderToList<TModel>(reader, out runTime, true);
                query.MapingTime += runTime;
            }
            else
            {
                list = MemoryDataCache.CacheService.GetCacheList<TModel>(sql, cacheTime, dbHelper, out cacheKey).Values.ToList();
            }
            ClearParame();
            query.RowCount = list.Count;
            SetOriginClone(list);
            return list;
        }
       
        
        #endregion



        internal override TType GetFunction<TType, TModel>(Expression<Func<TModel, bool>> expression, Expression<Func<TModel, TType>> selectField, FunctionType functionType, bool compileSp = false)
        {
            LambdaQuery<TModel> query = new RelationLambdaQuery<TModel>(dbContext, true);
            query.Select(selectField.Body);
            query.__FieldFunctionFormat = string.Format("{0}({1}) as Total", functionType, "{0}");
            query.Where(expression);
            var result = QueryScalar(query);
            if (result == null || result is DBNull)
            {
                return default(TType);
            }
            return (TType)result;
        }

        public override Dictionary<TKey, TValue> ToDictionary<TModel,TKey, TValue>(LambdaQuery<TModel> query)
        {
            var reader = GetQueryDynamicReader(query);
            return ObjectConvert.DataReadToDictionary<TKey, TValue>(reader);
        }
        
    }
}
