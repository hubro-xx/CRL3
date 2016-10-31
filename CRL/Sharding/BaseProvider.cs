/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace CRL.Sharding
{
    /// <summary>
    /// 分表数据管理实现
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class BaseProvider<TModel> : ProviderOrigin<TModel>
        where TModel : IModel, new()
    {
        /// <summary>
        /// 主数据索引
        /// </summary>
        int mainDataIndex;
        /// <summary>
        /// 使用主数据索引定位库
        /// </summary>
        /// <param name="_mainDataIndex"></param>
        public BaseProvider<TModel> SetLocation(int _mainDataIndex)
        {
            mainDataIndex = _mainDataIndex;
            var dataBase = DBService.GetDataBase(mainDataIndex);
            dbLocation.ShardingDataBase = dataBase;
            return this;
        }
        internal override DbContext GetDbContext()
        {
            if (SettingConfig.GetDbAccess == null)
            {
                throw new CRLException("请配置CRL数据访问对象,实现CRL.SettingConfig.GetDbAccess");
            }
            var helper = SettingConfig.GetDbAccess(dbLocation);
            var dbContext = new DbContext(helper, dbLocation);
            dbContext.ShardingMainDataIndex = mainDataIndex;
            dbContext.UseSharding = true;
            return dbContext;
        }
        /// <summary>
        /// 插入对象
        /// </summary>
        /// <param name="p"></param>
        public override void Add(TModel p)
        {
            //todo 判断主数据索引是不是在当前定位
            var dataIndex = Convert.ToInt32(p.GetpPrimaryKeyValue());
            SetLocation(dataIndex);
            base.Add(p);
        }
        public override void BatchInsert(List<TModel> list, bool keepIdentity = false)
        {
            throw new CRLException("暂不支持");
            //todo 判断主数据索引是不是在当前定位
            base.BatchInsert(list, keepIdentity);
        }
    }
}
