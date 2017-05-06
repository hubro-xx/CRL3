/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
namespace WebTest
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            CRL.Package.SettingConfig.OnlinePayOrderRefund = (order) =>
                {
                };
            //配置数据连接
            CRL.SettingConfig.GetDbAccess = (dbLocation) =>
            {
                var obj = dbLocation.TagData;
                if (dbLocation.ShardingDataBase != null)//按分库判断
                {
                    if (dbLocation.ShardingDataBase.Name == "db1")
                    {
                        return WebTest.Code.LocalSqlHelper.TestConnection;
                    }
                    else
                    {
                        return WebTest.Code.LocalSqlHelper.TestConnection2;
                    }
                }
                else
                {
                    //可按type区分数据库
                    var type2 = dbLocation.ManageType;
                    if (type2 == typeof(Code.MongoDBTestManage))
                    {
                        return Code.LocalSqlHelper.MongoDB;
                    }
                    return WebTest.Code.LocalSqlHelper.TestConnection;
                }
            };
            #region 缓存服务端实现
            //增加处理规则
            CRL.CacheServerSetting.AddCacheServerDealDataRule(typeof(Code.CacheDataTest), Code.CacheDataTestManage.Instance.DeaCacheCommand);
            //启动服务端
            var cacheServer = new CRL.CacheServer.TcpServer(11236);
            cacheServer.Start();
            #endregion

            //实现缓存客户端调用
            //有多个服务器添加多个
            //要使用缓存服务,需要设置ProductDataManage.QueryCacheFromRemote 为 true
            CRL.CacheServerSetting.AddTcpServerListen("127.0.0.1", 11236);
            //CRL.CacheServerSetting.AddTcpServerListen("122.114.91.203", 11236);
            CRL.CacheServerSetting.Init();
        }

    }
}
