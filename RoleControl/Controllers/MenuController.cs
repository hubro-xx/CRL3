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
    public class MenuController : RoleControl.Models.BaseController
    {
        //
        // GET: /Menu/

        public ActionResult Index(string parentCode = "")
        {
            List<CRL.Package.RoleAuthorize.Menu> list = new List<CRL.Package.RoleAuthorize.Menu>();
            int systemId = currentSystemId;
            list = CRL.Package.RoleAuthorize.MenuBusiness.Instance.QueryList(b => b.ParentCode == parentCode && b.DataType == systemId);
            ViewBag.Nav = CRL.Package.RoleAuthorize.MenuBusiness.Instance.GetParents(parentCode, systemId);
            return View(list.OrderByDescending(b=>b.Sort).ToList());
        }
        string GetUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";
            if (url.ToLower().IndexOf("http://") == -1)
            {
                return url;
            }
            Uri uri = new Uri(url);
            return uri.AbsolutePath;
        }
        [HttpPost]
        public ActionResult Add(string parentCode,CRL.Package.RoleAuthorize.Menu menu)
        {
            menu.Url = GetUrl(menu.Url).Trim(); ;
            menu.Name = menu.Name.Trim();
            menu.DataType = currentSystemId;
            CRL.Package.RoleAuthorize.MenuBusiness.Instance.Add(parentCode, menu);
            //return AutoBackResult("操作成功", Request.UrlReferrer.ToString());
            return RedirectToAction("Index", new { parentCode = parentCode });
        }
        [HttpPost]
        public ActionResult Update(CRL.Package.RoleAuthorize.Menu menu)
        {
            CRL.ParameCollection c = new CRL.ParameCollection();
            menu.Url = GetUrl(menu.Url);
            c["Name"] = menu.Name.Trim();
            c["Url"] = menu.Url.Trim();
            c["Disable"] = menu.Disable;
            c["ShowInNav"] = menu.ShowInNav;
            c["Sort"] = menu.Sort;
            CRL.Package.RoleAuthorize.MenuBusiness.Instance.Update(b => b.Id == menu.Id, c);
            //return AutoBackResult("操作成功", Request.UrlReferrer.ToString());
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult Delete(int id)
        {
            var menu = CRL.Package.RoleAuthorize.MenuBusiness.Instance.QueryItem(b => b.Id == id);
            string code = menu.SequenceCode;
            //删除下级
            CRL.Package.RoleAuthorize.MenuBusiness.Instance.Delete(menu.SequenceCode, currentSystemId);
            CRL.Package.RoleAuthorize.AccessControlBusiness.Instance.Delete(b => b.MenuCode == code);
            return Redirect(Request.UrlReferrer.ToString());
            return AutoBackResult("操作成功", Request.UrlReferrer.ToString());
        }
        public ActionResult Import()
        {
            int systemId = currentSystemId;
            var menus = CRL.Package.RoleAuthorize.MenuBusiness.Instance.QueryList(b => b.ParentCode == "" && b.DataType == systemId).OrderBy(b => b.SequenceCode);
            string str = "";
            foreach(var item in menus)
            {
                str += string.Format("{0} {1}\r\n",item.Name,item.Url);
                var childs = CRL.Package.RoleAuthorize.MenuBusiness.Instance.GetChild(item.SequenceCode, systemId);
                foreach (var item2 in childs)
                {
                    str += string.Format("\t{0} {1} {2}\r\n", item2.Name, item2.Url, item2.ShowInNav);
                }
            }
            ViewBag.MenuTxt = str;
            return View();
        }
        [HttpPost]
        public ActionResult Import(string txt)
        {
            txt = txt.Trim();
            string[] lines = txt.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int mainCode = 0;
            int childCode = 0;
            var menus = new List<CRL.Package.RoleAuthorize.Menu>();
            foreach(var s in lines)
            {
                string first=s.Substring(0,1);
                string[] arry = s.Trim().Split(' ');
                string url = "";
                bool showInNav = true;
                if (arry.Length > 1)
                {
                    url = arry[1].Trim();
                }
                if (arry.Length > 2)
                {
                    showInNav = Convert.ToBoolean(arry[2]);
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(first,@"\s"))//主菜单开始
                {
                    mainCode += 1;
                    childCode = 0;
                    menus.Add(new CRL.Package.RoleAuthorize.Menu() { Name = arry[0], SequenceCode = mainCode.ToString().PadLeft(2, '0'), DataType = currentSystemId, Url = url });
                }
                else
                {
                    childCode += 1;
                    string sequenceCode = mainCode.ToString().PadLeft(2, '0') + childCode.ToString().PadLeft(2, '0');
                    menus.Add(new CRL.Package.RoleAuthorize.Menu() { Name = arry[0], SequenceCode = sequenceCode, DataType = currentSystemId, Url = url, ParentCode = mainCode.ToString().PadLeft(2, '0'), ShowInNav = showInNav });
                }
            }
            CRL.Package.RoleAuthorize.MenuBusiness.Instance.Delete(b => b.DataType == currentSystemId);
            CRL.Package.RoleAuthorize.MenuBusiness.Instance.BatchInsert(menus);
            CRL.Package.RoleAuthorize.MenuBusiness.Instance.ClearCache();
            return Redirect("/Menu");
        }
    }
}
