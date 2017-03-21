/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Shopping.Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //初始数据访问
            CRL.SettingConfig.GetDbAccess = (type) =>
            {
                return new CoreHelper.SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["default"].ConnectionString);
            };

            var QuartzWorker = new CoreHelper.QuartzScheduler.QuartzWorker();
            var task = new BLL.ProxyPool.GetProxyJob();
            QuartzWorker.AddWork(task);
            //QuartzWorker.Start();

        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //检查票据,设置登录状态
            CoreHelper.FormAuthentication.AuthenticationSecurity.CheckTicket();
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Core.Mvc.CustomError.HandleError(Context);
            
        }

    }
}
