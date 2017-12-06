/**
* CRL 快速开发框架 V4.5
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.LambdaQuery;
using System.Linq.Expressions;
namespace CRL
{
    public static partial class ExtensionMethod
    {
        /// <summary>
        /// 按完整查询条件进行删除
        /// goup语法不支持
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static int Delete<T>(this LambdaQuery<T> query) where T : IModel, new()
        {
            var db = DBExtendFactory.CreateDBExtend(query.__DbContext);
            return db.Delete(query);
        }
        /// <summary>
        /// 按完整查询条件进行更新
        /// goup语法不支持
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        public static int Update<T>(this LambdaQuery<T> query, ParameCollection updateValue) where T : IModel, new()
        {
            var db = DBExtendFactory.CreateDBExtend(query.__DbContext);
            return db.Update(query, updateValue);
        }
        static DbContext getDbContext<T>()
        {
            var dbLocation = new CRL.DBLocation() { DataAccessType = DataAccessType.Default, ManageType = typeof(T) };
            var helper = SettingConfig.GetDbAccess(dbLocation);
            var dbContext = new DbContext(helper, dbLocation);
            return dbContext;
        }
        /// <summary>
        /// 保存更改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int Save<T>(this T obj) where T : IModel, new()
        {
            obj.CheckNull("obj");
            var dbContext = getDbContext<T>();
            var db = DBExtendFactory.CreateDBExtend(dbContext);
            return db.Update(obj);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int Delete<T>(this T obj) where T : IModel, new()
        {
            obj.CheckNull("obj");
            var dbContext = getDbContext<T>();
            var db = DBExtendFactory.CreateDBExtend(dbContext);
            return db.Delete<T>(obj.GetpPrimaryKeyValue());
        }
    }
}
