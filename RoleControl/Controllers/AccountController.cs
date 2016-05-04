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
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RoleControl.Models;

namespace RoleControl.Controllers
{
    [Authorize]
    public class AccountController : RoleControl.Models.BaseController
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            string adminName = CoreHelper.CustomSetting.GetConfigKey("adminName");
            string adminPass = CoreHelper.CustomSetting.GetConfigKey("adminPass");
            string saveIp = CoreHelper.CustomSetting.GetConfigKey("saveIp");
            string useSaveIp = CoreHelper.CustomSetting.GetConfigKey("useSaveIp");
            if (useSaveIp=="1")
            {
                if (CoreHelper.RequestHelper.IsRemote && Request.UserHostAddress != saveIp)
                {
                    ModelState.AddModelError("", "不安全的IP");
                    return View();
                }
            }
            if (model.Password != adminPass && model.UserName != adminName)
            {
                // 如果我们进行到这一步时某个地方出错，则重新显示表单
                ModelState.AddModelError("", "提供的用户名或密码不正确。");
                return View();
            }
            var count = CRL.Package.RoleAuthorize.SystemTypeBusiness.Instance.Count(b=>b.Id>0);
            if (count == 0)
            {
                CRL.Package.RoleAuthorize.SystemTypeBusiness.Instance.Add(new CRL.Package.RoleAuthorize.SystemType() { Name = "默认系统" });
            }
            CRL.Package.RoleAuthorize.Employee u = new CRL.Package.RoleAuthorize.Employee();
            u.Id = 0;
            u.Name = "admin";
            CRL.Package.RoleAuthorize.EmployeeBusiness.Instance.Login(u, "admin", false);
            CoreHelper.LocalCookie c = new CoreHelper.LocalCookie();
            c.Clear();
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/";
            }
            return Redirect(returnUrl);
        }

        //
        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}
