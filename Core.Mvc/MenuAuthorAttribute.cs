using System;

using System.Collections.Generic;

using System.Linq;

using System.Web;

using System.Web.Mvc;

using System.Xml.Linq;


namespace Core.Mvc
{
    /// <summary>
    /// 自定义菜单权限验证
    /// </summary>
    public class MenuAuthorAttribute : AuthorizeAttribute
    {
        static int CurrentSystemId
        {
            get
            {
                if (!CoreHelper.CustomSetting.ContainsKey("CurrentSystemId"))
                {
                    return 1;
                }
                return Convert.ToInt32(CoreHelper.CustomSetting.GetConfigKey("CurrentSystemId"));
            }
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            isMenuCheck = false;
            if (httpContext == null)
            {
                throw new ArgumentNullException("HttpContext");
            }
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }
            string userTicket = httpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userTicket))
                return false;

            var user = CRL.Package.Person.Person.ConverFromArry(userTicket);
            bool a = CRL.Package.RoleAuthorize.AccessControlBusiness.Instance.CheckAccess(CurrentSystemId, user.Id);
            //a = false;
            isMenuCheck = true;
            return a;
            //return base.AuthorizeCore(httpContext);
        }
        bool isMenuCheck = false;

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var Request = filterContext.HttpContext.Request;
            string url = Request.Url.Scheme + "://" + Request.Url.Authority + System.Web.Security.FormsAuthentication.LoginUrl;
            if (Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult() { Data = new { Result = false, Message = "没有权限进行此操作,请联系管理员分配权限" } };
                return;
            }
            if (isMenuCheck)
            {
                var message = string.Format("没有权限进行此操作,请联系管理员分配权限 \n地址:[{0}]", Request.Path);
                filterContext.Result = new ContentResult() { Content = message };
            }
            else
            {
                url += string.Format("?returnUrl={0}", System.Web.HttpUtility.UrlEncode(Request.Url.ToString()));
                filterContext.Result = new RedirectResult(url);
            }
        }

    }

}