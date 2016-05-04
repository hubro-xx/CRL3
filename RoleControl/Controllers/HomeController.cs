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
using System.Web.Mvc;

namespace RoleControl.Controllers
{
    [Authorize]
    public class HomeController : RoleControl.Models.BaseController
    {
        public ActionResult Test()
        {
            return Content("");
        }
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }
        [HttpPost]
        public ActionResult Index(string a)
        {
            //string msg = CRL.Package.RoleAuthorize.AccessControlBusiness.Instance.CreateTable();
            var count = CRL.Package.RoleAuthorize.MenuBusiness.Instance.Count(b => b.Id > 0);
            if (count == 0)
            {
                //菜单
                var menu = new CRL.Package.RoleAuthorize.Menu();
                menu.SequenceCode = "01";
                menu.Name = "首页";
                menu.ShowInNav = true;
                menu.DataType = 1;
                menu.ParentCode = "";
                CRL.Package.RoleAuthorize.MenuBusiness.Instance.Add(menu);

                menu = new CRL.Package.RoleAuthorize.Menu();
                menu.SequenceCode = "0101";
                menu.Name = "测试页";
                menu.DataType = 1;
                menu.ShowInNav = true;
                menu.Url = "/Demo/List";
                menu.ParentCode = "01";
                CRL.Package.RoleAuthorize.MenuBusiness.Instance.Add(menu);

                menu = new CRL.Package.RoleAuthorize.Menu();
                menu.SequenceCode = "0102";
                menu.Name = "测试页提交";
                menu.DataType = 1;
                menu.ShowInNav = false;
                menu.Url = "/Demo/Update";
                menu.ParentCode = "01";
                CRL.Package.RoleAuthorize.MenuBusiness.Instance.Add(menu);

                //角色
                var role = new CRL.Package.RoleAuthorize.Role();
                role.Name = "管理员";
                CRL.Package.RoleAuthorize.RoleBusiness.Instance.Add(role);
                //权限
                var control = new CRL.Package.RoleAuthorize.AccessControl();
                control.MenuCode = "0101";
                control.SystemTypeId = 1;
                control.Que = true;
                control.Role = 1;
                control.RoleType = CRL.Package.RoleAuthorize.RoleType.角色;
                CRL.Package.RoleAuthorize.AccessControlBusiness.Instance.Add(control);

                control = new CRL.Package.RoleAuthorize.AccessControl();
                control.MenuCode = "0102";
                control.SystemTypeId = 1;
                control.Que = false;
                control.Role = 1;
                control.RoleType = CRL.Package.RoleAuthorize.RoleType.角色;
                CRL.Package.RoleAuthorize.AccessControlBusiness.Instance.Add(control);
                //用户
                CRL.Package.RoleAuthorize.Employee u = new CRL.Package.RoleAuthorize.Employee();
                string name = "test";
                string pass = "test";
                u.AccountNo = name;
                u.Role = 1;
                u.PassWord = CRL.Package.RoleAuthorize.EmployeeBusiness.Instance.EncryptPass(pass);
                u.Name = name;
                CRL.Package.RoleAuthorize.EmployeeBusiness.Instance.Add(u);
            }
            return AutoBackResult("完成", Request.UrlReferrer.ToString());
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
        public ActionResult ChangeSystem(int id)
        {
            RoleControl.Models.BaseController.currentSystemId = id;
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
