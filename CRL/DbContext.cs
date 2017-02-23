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
using System.Text;

namespace CRL
{
    /// <summary>
    /// 数据访问上下文
    /// </summary>
    public class DbContext
    {
        /// <summary>
        /// 数据访问上下文
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="dbLocation"></param>
        public DbContext(CoreHelper.DBHelper dbHelper, DBLocation dbLocation)
        {
            DBHelper = dbHelper;
            DBLocation = dbLocation;
            //todo 按数据库类型类型判断
            DataBaseArchitecture = dbHelper.CurrentDBType == CoreHelper.DBType.MongoDB ? DataBaseArchitecture.NotRelation : CRL.DataBaseArchitecture.Relation;
        }
        /// <summary>
        /// 数据库架构类型
        /// </summary>
        internal DataBaseArchitecture DataBaseArchitecture;
        /// <summary>
        /// 数据库连接定位
        /// </summary>
        internal DBLocation DBLocation;
        /// <summary>
        /// 数据访问
        /// </summary>
        internal CoreHelper.DBHelper DBHelper;
        /// <summary>
        /// 分库表定位索引
        /// 大于0则按需要查找分表判断
        /// </summary>
        internal int ShardingMainDataIndex = 0;
        /// <summary>
        /// 是否使用分表定位
        /// </summary>
        public bool UseSharding = false;

        /// <summary>
        /// 当前查询参数索引
        /// </summary>
        internal int parIndex = 0;
        public string Name;
        public override string ToString()
        {
            return Name;
        }
    }
    /// <summary>
    /// 数据库连接定位
    /// 通过判断Type或DataBase判断数据连接
    /// 优先ShardingDataBase判断
    /// </summary>
    public class DBLocation
    {
        /// <summary>
        /// 调用的类型
        /// </summary>
        public Type ManageType;
        /// <summary>
        /// 附加的定位数据
        /// </summary>
        public object TagData;
        /// <summary>
        /// 分库指定的数据库
        /// </summary>
        public Sharding.DB.DataBase ShardingDataBase;
    }
}
