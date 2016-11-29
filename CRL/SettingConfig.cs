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
using System.Threading.Tasks;

namespace CRL
{
  

    /// <summary>
    /// 表示CacheServer处理数据的方法委托
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public delegate CacheServer.ResultData ExpressionDealDataHandler(CacheServer.Command command);

    /// <summary>
    /// as bool TransMethod(out string error);
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public delegate bool TransMethod(out string error);

    /// <summary>
    /// 框架部署,请实现委托
    /// </summary>
    public class SettingConfig
    {
        #region 委托
        /// <summary>
        /// 获取数据连接
        /// </summary>
        public static Func<DBLocation, CoreHelper.DBHelper> GetDbAccess;
        //public static GetShardingDbHandler GetShardingDbAccess;

        /// <summary>
        /// 暂存虚拟字段格式化方法
        /// </summary>
        public static Func<string, string> StringFormat;

        
        #endregion

        /// <summary>
        /// 清除所有内置缓存
        /// </summary>
        public static void ClearCache()
        {
            MemoryDataCache.CacheService.Clear();
        }
        /// <summary>
        /// 是否使用属性更改通知
        /// 如果使用了,在查询时就不设置源对象克隆
        /// 在实现了属性构造后,可设为true
        /// </summary>
        public static bool UsePropertyChange = false;

        /// <summary>
        /// 是否检测表结构,生产服务器可将此值设为FALSE
        /// </summary>
        public static bool CheckModelTableMaping = true;

        /// <summary>
        /// 是否自动跟踪对象状态
        /// 为否则需要调用IMode.BeginTracking(),使更新时能识别
        /// query.__TrackingModel,同时生效,才进行跟踪
        /// </summary>
        public static bool AutoTrackingModel = true;

    }


}
