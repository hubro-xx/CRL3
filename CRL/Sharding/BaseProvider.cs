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
                throw new Exception("请配置CRL数据访问对象,实现CRL.SettingConfig.GetDbAccess");
            }
            var helper = SettingConfig.GetDbAccess(dbLocation);
            var dbContext = new DbContext(helper, dbLocation);
            dbContext.ShardingMainDataIndex = mainDataIndex;
            dbContext.UseSharding = true;
            return dbContext;
        }
    }
}
