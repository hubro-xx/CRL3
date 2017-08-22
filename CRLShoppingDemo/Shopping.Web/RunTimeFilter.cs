using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Shopping.Web
{
    public class RunTimeFilter : ActionFilterAttribute
    {
        Stopwatch sw;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            sw = new Stopwatch();
            sw.Start();
            var controllerName = (filterContext.RouteData.Values["controller"]).ToString().ToLower();
            var actionName = (filterContext.RouteData.Values["action"]).ToString().ToLower();
            var areaName = (filterContext.RouteData.DataTokens["area"] == null ? "" : filterContext.RouteData.DataTokens["area"]).ToString().ToLower();

            var name = string.Format("/{0}/{1}/{2}", areaName, controllerName, actionName);
            CRL.Runtime.RunTimeService.BeginLog(name);
            base.OnActionExecuting(filterContext);
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            sw.Stop();
            var el = sw.ElapsedMilliseconds;
            var controllerName = (filterContext.RouteData.Values["controller"]).ToString().ToLower();
            var actionName = (filterContext.RouteData.Values["action"]).ToString().ToLower();
            var areaName = (filterContext.RouteData.DataTokens["area"] == null ? "" : filterContext.RouteData.DataTokens["area"]).ToString().ToLower();

            var name = string.Format("/{0}/{1}/{2}", areaName, controllerName, actionName);
            CRL.Runtime.RunTimeService.Log(name, el);
        }
    }
}