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
        #region insert
        /// <summary>
        /// 批量插入,并指定是否保持自增主键
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="details"></param>
        /// <param name="keepIdentity"></param>
        public override void BatchInsert<TModel>(List<TModel> details,bool keepIdentity=false)
        {
            CheckTableCreated<TModel>();
            if (details.Count == 0)
                return;
            foreach (TModel item in details)
            {
                //item.CheckRepeatedInsert = false;
                CheckData(item);
            }
            _DBAdapter.BatchInsert(details, keepIdentity);
            //var type = typeof(TModel);
            //if (TypeCache.ModelKeyCache.ContainsKey(type))
            //{
            //    string key = TypeCache.ModelKeyCache[type];
            //    foreach (var item in details)
            //    {
            //        MemoryDataCache.UpdateCacheItem(key, item);
            //    }
            //}
            var updateModel = MemoryDataCache.CacheService.GetCacheTypeKey(typeof(TModel));
            foreach (var item in details)
            {
                foreach (var key in updateModel)
                {
                    MemoryDataCache.CacheService.UpdateCacheItem(key, item, null);
                }
            }
        }
       
        /// <summary>
        /// 单个插入
        /// </summary>
        /// <param name="obj"></param>
        public override void InsertFromObj<TModel>(TModel obj)
        {
            //var Reflection = ReflectionHelper.GetInfo<TModel>();
            CheckTableCreated<TModel>();
            var primaryKey = TypeCache.GetTable(obj.GetType()).PrimaryKey;
            CheckData(obj);
            var index = _DBAdapter.InsertObject(obj);
            if (!primaryKey.KeepIdentity)
            {
                //Reflection.GetAccessor(primaryKey.Name).Set((TModel)obj, index);
                index = ObjectConvert.ConvertObject(primaryKey.PropertyType, index);
                primaryKey.SetValue(obj, index);
            }
            ClearParame();
            var clone = obj.Clone();
            obj.OriginClone = clone as TModel;
            //var type = typeof(TModel);
            //if (TypeCache.ModelKeyCache.ContainsKey(type))
            //{
            //    string key = TypeCache.ModelKeyCache[type];
            //    MemoryDataCache.UpdateCacheItem(key, obj);
            //}
            UpdateCacheItem<TModel>(obj, null, true);
            //return index;
        }
        #endregion
    }
}
