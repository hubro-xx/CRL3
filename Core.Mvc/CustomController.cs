/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace Core.Mvc
{
    public class CustomController
    {
        public static void Execute(System.Web.HttpContext context, string controller, string action, Dictionary<string, object> parame)
        {
            var routeData = new RouteData();
            routeData.Values.Add("controller", controller);
            routeData.Values.Add("action", action);
            foreach (var e in parame.Keys)
            {
                routeData.Values.Add(e, parame[e]);
            }
            var currentController = ControllerBuilder.Current.GetControllerFactory().CreateController(
                new RequestContext(new HttpContextWrapper(context), routeData), controller);

            currentController.Execute(new RequestContext(new HttpContextWrapper(context), routeData));
        }
    }
}
