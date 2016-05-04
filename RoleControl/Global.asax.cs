/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RoleControl
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //AuthConfig.RegisterAuth();

            CRL.SettingConfig.GetDbAccess = (type) =>
            {
                var connString = ConnectionManage.GetCurrent();
                return new CoreHelper.SqlHelper(connString);

                string dbName = CoreHelper.CustomSetting.GetConfigKey("dbName");
                return CreateDbHelper(dbName);
            };
            //重写密码加密方法
            //CRL.SettingConfig.RoleAuthorizeEncryptPass = (pass) =>
            //{
            //    return pass;
            //};
        }
        public static CoreHelper.SqlHelper CreateDbHelper(string name)
        {
            //默认在网站根目录/DBConnection,如果没有,则在D盘找
            string connString = CoreHelper.CustomSetting.GetConnectionString(name);
            var m = System.Text.RegularExpressions.Regex.Match(connString, @"Source\s*\=(.+?);", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string server = m.Groups[1].ToString().ToLower();
            if (!CoreHelper.RequestHelper.IsRemote)
            {
                if (server.IndexOf("192.168.") == -1 && server.IndexOf("127.0.") == -1 && server.IndexOf("localhost") == -1)
                {
                    throw new Exception("本地程序不能调用网上数据库:" + server);
                }
            }
            var help = new CoreHelper.SqlHelper(connString);
            return help;
        }
    }
}
