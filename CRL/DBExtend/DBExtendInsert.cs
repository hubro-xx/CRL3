/**
* CRL 快速开发框架 V3.1
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
namespace CRL
{
    public sealed partial class DBExtend
    {
        #region insert
        /// <summary>
        /// 批量插入,并指定是否保持自增主键
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="details"></param>
        /// <param name="keepIdentity"></param>
        public void BatchInsert<TItem>(List<TItem> details,bool keepIdentity=false) where TItem : IModel,new()
        {
            CheckTableCreated<TItem>();
            if (details.Count == 0)
                return;
            foreach (TItem item in details)
            {
                //item.CheckRepeatedInsert = false;
                CheckData(item);
            }
            _DBAdapter.BatchInsert(details, keepIdentity);
            //var type = typeof(TItem);
            //if (TypeCache.ModelKeyCache.ContainsKey(type))
            //{
            //    string key = TypeCache.ModelKeyCache[type];
            //    foreach (var item in details)
            //    {
            //        MemoryDataCache.UpdateCacheItem(key, item);
            //    }
            //}
            var updateModel = MemoryDataCache.CacheService.GetCacheTypeKey(typeof(TItem));
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
        public void InsertFromObj<TItem>(TItem obj) where TItem : IModel, new()
        {
            CheckTableCreated<TItem>();
            var primaryKey = TypeCache.GetTable(obj.GetType()).PrimaryKey;
            CheckData(obj);
            var index = _DBAdapter.InsertObject(obj);
            if (!primaryKey.KeepIdentity)
            {
                primaryKey.SetValue(obj, index);
            }
            ClearParame();
            var clone = obj.Clone();
            obj.OriginClone = clone as TItem;
            //var type = typeof(TItem);
            //if (TypeCache.ModelKeyCache.ContainsKey(type))
            //{
            //    string key = TypeCache.ModelKeyCache[type];
            //    MemoryDataCache.UpdateCacheItem(key, obj);
            //}
            UpdateCacheItem<TItem>(obj, null, true);
            //return index;
        }
        #endregion
    }
}
