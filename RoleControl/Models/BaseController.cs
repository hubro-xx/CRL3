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
using System.Web.Mvc;

namespace RoleControl.Models
{
    public class BaseController : System.Web.Mvc.Controller
    {
        public static int currentSystemId
        {
            get
            {
                CoreHelper.LocalCookie c = new CoreHelper.LocalCookie();
                var v = c["currentSystemId"];
                if (string.IsNullOrEmpty(v))
                {
                    int a= CRL.Package.RoleAuthorize.SystemTypeBusiness.Instance.SystemTypes[0].Id;
                    c["currentSystemId"] = a.ToString();
                    return a;
                }
                return Convert.ToInt32(v);
            }
            set
            {
                CoreHelper.LocalCookie c = new CoreHelper.LocalCookie();
                c["currentSystemId"] = value.ToString() ;
            }
        }
        #region object
        /// <summary>
        /// 处理结果
        /// </summary>
        public class DealResult
        {
            /// <summary>
            /// 处理结果
            /// </summary>
            public bool Result
            {
                get;
                set;
            }
            /// <summary>
            /// 消息
            /// </summary>
            public string Message
            {
                get;
                set;
            }
            /// <summary>
            /// 关联数据
            /// </summary>
            public object Data
            {
                get;
                set;
            }
        }
        #endregion

        /// <summary>
        /// 自动返回上一页
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public System.Web.Mvc.ActionResult AutoBackResult(string message)
        {
            return AutoBackResult(message, "");
        }
        /// <summary>
        /// 自动返回到指定URL
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public System.Web.Mvc.ActionResult AutoBackResult(string message, string url)
        {
            if (string.IsNullOrEmpty(message))
                message = "操作成功";
            string backJs = "window.location.href ='" + url + @"';";
            if (string.IsNullOrEmpty(url))//URL为空则后退
            {
                backJs = "window.history.back();";
            }
            string html = @"<script type='text/javascript' reload='1'>
function goUrl()
{
    " + backJs + @"
}
setTimeout('goUrl()', 2300)</script>
<a href='javascript:goUrl()'>如果您的浏览器没有自动跳转，请点击此链接</a>";
            //html = string.Format(html, url, message);
            return PageContent(message, html);
        }
        /// <summary>
        /// 根据METHOD判断跳转还是返回TRUE
        /// </summary>
        /// <returns></returns>
        public System.Web.Mvc.ActionResult AutoReturn(string message)
        {
            if (Request.RequestType == "GET")
            {
                return AutoBackResult(message, Request.UrlReferrer + "");
            }
            else
            {
                return JsonResult(true, message);
            }
        }
        /// <summary>
        /// 输出继承布局的Content
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ActionResult PageContent(string title, string content)
        {
            var result = new CustomActionResult("~/views/shared/PageContent.cshtml");
            TempData["content"] = content;
            TempData["title"] = title;
            result.TempData = TempData;
            return result;
        }
        public ActionResult PageContent(string title)
        {
            return PageContent(title, "<a href='javascript:history.back()'>返回上一页</a>");
        }
        /// <summary>
        /// 返回封装好的Json结果
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public System.Web.Mvc.JsonResult JsonResult(bool result, string message, object data)
        {
            DealResult obj = new DealResult() { Result = result, Message = message, Data = data };
            return Json(obj);
        }
        /// <summary>
        /// 返回封装好的Json结果
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public System.Web.Mvc.JsonResult JsonResult(bool result, string message)
        {
            return JsonResult(result, message, "");
        }
    }
}
