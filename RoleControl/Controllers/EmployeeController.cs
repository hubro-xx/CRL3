/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using RoleControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL;
namespace RoleControl.Controllers
{
    [Authorize]
    public class EmployeeController : RoleControl.Models.BaseController
    {
        //
        // GET: /User/

        public ActionResult Index(string name,int role=0,int page=1)
        {
            int count;
            int pageSize = 15;
            var query = CRL.Package.RoleAuthorize.EmployeeBusiness.Instance.GetLambdaQuery();
            if (!string.IsNullOrEmpty(name))
            {
                query.Where(b => b.Name.Like("%" + name + "%"));
            }
            if (role > 0)
            {
                query.Where(b=>b.Role==role);
            }
            query.Page(pageSize, page);
            query.OrderBy(b => b.Id, true);
            //todo 分页
            var list = query.ToList();
            count = query.RowCount;
            var pageObj = new PageObj<CRL.Package.RoleAuthorize.Employee>(list, page, count, pageSize);
            var roles = CRL.Package.RoleAuthorize.RoleBusiness.Instance.QueryList();
            //var selectList = new SelectList(roles, "id", "Name", "24");
            ViewBag.Role = roles;
            return View(pageObj);
        }
        public ActionResult Add(string Name, string PassWord, int role, string departmentCode)
        {
            CRL.Package.RoleAuthorize.Employee u = new CRL.Package.RoleAuthorize.Employee();
            u.AccountNo = Name;
            u.Role = role;
            if (CRL.Package.RoleAuthorize.EmployeeBusiness.Instance.CheckAccountExists(Name))
            {
                return AutoBackResult("帐号已存在");
            }
            u.PassWord = CRL.Package.RoleAuthorize.EmployeeBusiness.Instance.EncryptPass(PassWord);
            u.Department = departmentCode;
            u.Name = Name;
            CRL.Package.RoleAuthorize.EmployeeBusiness.Instance.Add(u);
            return Redirect("/Employee/");
            return AutoBackResult("操作成功", "/Employee/detail?id=" + u.Id);
        }
        /// <summary>
        /// 添加员工头像
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="uploadType"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult Update(CRL.Package.RoleAuthorize.Employee u)
        {
            CRL.ParameCollection c = new CRL.ParameCollection();
            if (!string.IsNullOrEmpty(u.PassWord))
            {
                c["PassWord"] = CRL.Package.RoleAuthorize.EmployeeBusiness.Instance.EncryptPass(u.PassWord);
            }
            c["Locked"] = u.Locked;
            c["Role"] = u.Role;
            c["Department"] = u.Department;
            CRL.Package.RoleAuthorize.EmployeeBusiness.Instance.Update(b => b.Id == u.Id, c);
            return Redirect(Request.UrlReferrer.ToString());
            return AutoBackResult("操作成功", Request.UrlReferrer.ToString());
        }
        public ActionResult Delete(int id)
        {
            CRL.Package.RoleAuthorize.EmployeeBusiness.Instance.Delete(b => b.Id == id);
            CRL.Package.RoleAuthorize.AccessControlBusiness.Instance.Delete(b => b.Role == id && b.RoleType == CRL.Package.RoleAuthorize.RoleType.用户);
            return Redirect(Request.UrlReferrer.ToString());
            return AutoBackResult("操作成功", Request.UrlReferrer.ToString());
        }
        public ActionResult Detail(int id)
        {
            var user = CRL.Package.RoleAuthorize.EmployeeBusiness.Instance.QueryItem(b => b.Id == id);
            var menus = CRL.Package.RoleAuthorize.MenuBusiness.Instance.GetUserMenu(id, RoleControl.Models.BaseController.currentSystemId, false);
            ViewBag.Menus = menus;
            return View(user);
        }
        [HttpPost]
        public ActionResult Detail(CRL.Package.RoleAuthorize.Employee u)
        {
            CRL.ParameCollection c = new CRL.ParameCollection();
            c["Name"] = u.Name;
            //c["AccountNo"] = u.Name;
            c["Mobile"] = u.Mobile;
            c["qq"] = u.QQ;
            c["Email"] = u.Email;
            c["Birthday"] = u.Birthday;
            c["Sex"] = u.Sex;
            c["IdentityNo"] = u.IdentityNo;
            c["Address"] = u.Address;
            c["RegisterIp"] = u.RegisterIp;
            c["Department"] = u.Department;
            CRL.Package.RoleAuthorize.EmployeeBusiness.Instance.Update(b => b.Id == u.Id, c);
            CRL.Package.RoleAuthorize.AccessControlBusiness.Instance.Delete(b => b.Role == u.Id && b.RoleType == CRL.Package.RoleAuthorize.RoleType.用户);
            //return Redirect("/Employee/");
            return AutoBackResult("操作成功", Request.UrlReferrer.ToString());
        }
    }
}
