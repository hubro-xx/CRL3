using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Core.Mvc
{
    public class CustomError
    {
        /// <summary>
        /// 处理输出错误
        /// </summary>
        /// <param name="context"></param>
        public static void HandleError(HttpContext context)
        {
            //if (!context.IsCustomErrorEnabled)
            //{
            //    return;
            //}
            var exception = context.Server.GetLastError();
            var httpException = new HttpException(null, exception);

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Home");
            routeData.Values.Add("action", "_CustomError");
            routeData.Values.Add("httpException", httpException);
            string errorCode = CoreHelper.ExceptionHelper.InnerLogException(exception);
            routeData.Values.Add("errorCode", errorCode);

            context.Server.ClearError();
            var errorController = ControllerBuilder.Current.GetControllerFactory().CreateController(
                new RequestContext(new HttpContextWrapper(context), routeData), "Home");

            errorController.Execute(new RequestContext(new HttpContextWrapper(context), routeData));
        }
    }
}
