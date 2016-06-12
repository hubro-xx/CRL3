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
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CRL;

namespace Core.Mvc
{
    public class CustomError
    {
        static Exception GetInnerException(Exception exp)
        {
            if (exp.InnerException != null)
            {
                exp = exp.InnerException;
                return GetInnerException(exp);
            }
            return exp;
        }
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
            string errorCode = CoreHelper.ExceptionHelper.InnerLogException(exception);
            if (new HttpRequestWrapper(context.Request).IsAjaxRequest())
            {
                var result = new BaseController.DealResult() { Result = false, Data = "服务器内部错误:" + exception.Message };
                context.Response.ContentType = "application/json";
                context.Response.Write(CoreHelper.SerializeHelper.SerializerToJson(result));
                context.Response.End();
                return;
            }
            string html = Properties.Resources.erro;
            html = html.Replace("[charset]", context.Request.ContentEncoding.WebName);
            if (exception != null)
            {
                string erroDetail = exception.StackTrace + "";
                erroDetail = erroDetail.Replace("\r\n", "<br>");
                erroDetail = context.Server.HtmlEncode(erroDetail);
                html = html.Replace("[TIME]", DateTime.Now.ToString());
                html = html.Replace("[ERRO_CODE]", errorCode);
                html = html.Replace("[URL]", context.Request.Url.ToString());
                html = html.Replace("[ERRO_TITLE]", context.Server.HtmlEncode(exception.Message));
                html = html.Replace("[ERRO_MESSAGE]", erroDetail);
            }
            context.Server.ClearError();
            context.Response.ContentType = "text/html";
            context.Response.Write(html);
            context.Response.End();
            return;
        }
    }
}
