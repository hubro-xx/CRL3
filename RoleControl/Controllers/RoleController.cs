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
    public class RoleController : RoleControl.Models.BaseController
    {
        //
        // GET: /Role/

        public ActionResult Index()
        {
            var list = CRL.Package.RoleAuthorize.RoleBusiness.Instance.QueryList();
            return View(list);
        }
        public ActionResult Add(CRL.Package.RoleAuthorize.Role role)
        {
            CRL.Package.RoleAuthorize.RoleBusiness.Instance.Add(role);
            CRL.Package.RoleAuthorize.RoleBusiness.Instance.ClearCache();
            return Redirect(Request.UrlReferrer.ToString());
            return AutoBackResult("操作成功", Request.UrlReferrer.ToString());
        }
        public ActionResult Update(CRL.Package.RoleAuthorize.Role role)
        {
            CRL.ParameCollection c = new CRL.ParameCollection();
            c["Name"] = role.Name;
            c["Remark"] = role.Remark;
            CRL.Package.RoleAuthorize.RoleBusiness.Instance.Update(b => b.Id == role.Id, c);
            return Redirect(Request.UrlReferrer.ToString());
            return AutoBackResult("操作成功", Request.UrlReferrer.ToString());
        }
        public ActionResult Delete(int id)
        {
            CRL.Package.RoleAuthorize.RoleBusiness.Instance.Delete(b => b.Id == id);
            CRL.Package.RoleAuthorize.AccessControlBusiness.Instance.Delete(b => b.Role == id && b.RoleType == CRL.Package.RoleAuthorize.RoleType.角色);
            CRL.Package.RoleAuthorize.RoleBusiness.Instance.ClearCache();
            return Redirect(Request.UrlReferrer.ToString());
            return AutoBackResult("操作成功", Request.UrlReferrer.ToString());
        }
    }
}
