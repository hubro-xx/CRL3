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
using CRL;
using CRL.Package.RoleAuthorize;
namespace RoleControl.Controllers
{
    [Authorize]
    public class AccessControlController : RoleControl.Models.BaseController
    {
        public ActionResult SetAccess(int roleId=0, int userId = 0)
        {
            int systemId = currentSystemId;
            var menus = CRL.Package.RoleAuthorize.MenuBusiness.Instance.QueryList(b => b.DataType == systemId);
            //初始值
            menus.ForEach(b => {
                b.Enabled = true;
            });
            List<AccessControl> controls;
            if (userId == 0)
            {
                var role = RoleBusiness.Instance.QueryItem(b => b.Id == roleId);
                controls = CRL.Package.RoleAuthorize.AccessControlBusiness.Instance.QueryList(b => b.Role == roleId && b.RoleType == RoleType.角色 && b.SystemTypeId == systemId);
                ViewBag.Title = string.Format("设置角色 [{0}] 的权限", role.Name);
            }
            else//查询包括用户设置
            {
                var u = EmployeeBusiness.Instance.QueryItem(b => b.Id == userId);
                var role = RoleBusiness.Instance.QueryItem(b => b.Id == u.Role);
                controls = CRL.Package.RoleAuthorize.AccessControlBusiness.Instance.QueryList(b => ((b.Role == userId && b.RoleType == RoleType.用户) || (b.Role == u.Role && b.RoleType == RoleType.角色)) && b.SystemTypeId == systemId);
                ViewBag.Title = string.Format("设置用户 [{0}] 的权限,所属角色 [{1}]", u.Name,role.Name);
            }
            foreach (var item in controls)
            {
                var m = menus.Where(b => b.SequenceCode == item.MenuCode).FirstOrDefault();
                if (m == null)
                    continue;
                //如果是角色设置的,则为只读
                //只能控制整个菜单的只读
                if (userId > 0 && item.RoleType == RoleType.角色)
                {
                    m.Enabled = false;
                }
                else
                {
                    m.Enabled = true;
                }
                m.Que = item.Que;
            }

            Dictionary<CRL.Package.RoleAuthorize.Menu, List<CRL.Package.RoleAuthorize.Menu>> dic = new Dictionary<CRL.Package.RoleAuthorize.Menu, List<CRL.Package.RoleAuthorize.Menu>>();
            CRL.Package.RoleAuthorize.Menu lastMenu = null;
            menus = menus.OrderBy(b=>b.SequenceCode).ToList();
            foreach (var item in menus)
            {
                if (item.SequenceCode.Length == 2)
                {
                    lastMenu = item;
                    dic.Add(item, new List<CRL.Package.RoleAuthorize.Menu>());
                }
                else
                {
                    dic[lastMenu].Add(item);
                }
            }
            dic = dic.OrderByDescending(b => b.Key.Sort).ToDictionary(b => b.Key, c => c.Value.OrderByDescending(d => d.Sort).ToList());
            return View(dic);
        }
        /// <summary>
        /// 通过脚本设置
        /// </summary>
        /// <param name="menuCode"></param>
        /// <param name="acc"></param>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetItemAccess(string menuCode,bool acc, int roleId = 0, int userId = 0)
        {
            int systemId = currentSystemId;
            foreach (string str in menuCode.Split(','))
            {
                string s = str.Trim();
                CRL.Package.RoleAuthorize.AccessControl item = new CRL.Package.RoleAuthorize.AccessControl();
                item.SystemTypeId = systemId;
                if (userId == 0)
                {
                    item.Role = roleId;
                    item.RoleType = RoleType.角色;
                }
                else
                {
                    item.Role = userId;
                    item.RoleType = RoleType.用户;
                }
                item.Que = acc;
                item.MenuCode = s.Trim();

                AccessControl item2;
                if (userId == 0)
                {
                    item2 = AccessControlBusiness.Instance.QueryItem(b => b.Role == roleId && b.RoleType == RoleType.角色 && b.SystemTypeId == systemId && b.MenuCode == s);
                }
                else
                {
                    item2 = AccessControlBusiness.Instance.QueryItem(b => b.Role == userId && b.RoleType == RoleType.用户 && b.SystemTypeId == systemId && b.MenuCode == s);
                }
                if (item2 == null)
                {
                    AccessControlBusiness.Instance.Add(item);
                }
                else
                {
                    ParameCollection c = new ParameCollection();
                    c["Que"] = item.Que;
                    AccessControlBusiness.Instance.Update(b => b.Id == item2.Id, c);
                }
            }
            return JsonResult(true,"");
        }
        [HttpPost]
        public ActionResult SetAccess(string[] selectOperate, int roleId=0, int userId = 0)
        {
            if (selectOperate == null)
            {
                return AutoBackResult("失败,选择菜单为空");
            }
            List<AccessControl> list = new List<AccessControl>();
            int systemId = currentSystemId;
            foreach (string s in selectOperate)
            {
                CRL.Package.RoleAuthorize.AccessControl item = new CRL.Package.RoleAuthorize.AccessControl();
                bool insert = false;
                if(Request["Query_" + s] != null)
                {
                    insert = true;
                    item.Que=true;
                }
                
                if (userId == 0)
                {
                    item.Role = roleId;
                    item.RoleType = RoleType.角色;
                }
                else
                {
                    item.Role = userId;
                    item.RoleType = RoleType.用户;
                }
                item.MenuCode =s;
                if (insert)
                {
                    item.SystemTypeId = systemId;
                    list.Add(item);
                }
            }
            
            if (userId == 0)
            {
                AccessControlBusiness.Instance.Delete(b => b.Role == roleId && b.RoleType == RoleType.角色 && b.SystemTypeId == systemId);
            }
            else
            {
                AccessControlBusiness.Instance.Delete(b => b.Role == userId && b.RoleType == RoleType.用户 && b.SystemTypeId == systemId);
            }
            AccessControlBusiness.Instance.BatchInsert(list);
            return AutoBackResult("设置成功",Request.UrlReferrer.ToString());
        }
    }
}
