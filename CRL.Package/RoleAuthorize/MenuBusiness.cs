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

namespace CRL.Package.RoleAuthorize
{
    public class MenuBusiness : Category.CategoryBusiness<Menu>
    {
        public static MenuBusiness Instance
        {
            get { return new MenuBusiness(); }
        }

        /// <summary>
        /// 返回菜单码
        /// </summary>
        /// <param name="systemTypeId"></param>
        /// <returns></returns>
        public string GetMenuCodeByUrl(int systemTypeId)
        {
            var item = GetMenuByUrl(systemTypeId);
            if (item == null)
                return "";
            return item.SequenceCode;
        }
        public Menu GetMenuByUrl(int systemTypeId, string url = "")
        {
            if (string.IsNullOrEmpty(url))
            {
                url = System.Web.HttpContext.Current.Request.Path;
            }
            url = url.ToLower();
            var allCache = GetAllCache(systemTypeId);
            var items = allCache.Where(b => b.DataType == systemTypeId
                && !string.IsNullOrEmpty(b.Url)
                && url == b.Url.ToLower()
                && b.ParentCode != "");
            return items.FirstOrDefault();
        }
        /// <summary>
        /// 查询用户的菜单
        /// </summary>
        /// <param name="systemTypeId"></param>
        /// <param name="userId"></param>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public List<Menu> GetUserMenu(int systemTypeId,int userId, int userRole)
        {
            List<Menu> menus = new List<Menu>();
            var controls = AccessControlBusiness.Instance.QueryList(b => b.Role == userRole || (b.RoleType == RoleType.用户 && b.Role == userId) && b.SystemTypeId == systemTypeId);
            var allCache = GetAllCache(systemTypeId);
            menus.AddRange(allCache.Where(b => b.SequenceCode.Length == 2));
            foreach (var item in controls)
            {
                Menu m = Get(item.MenuCode, 0);
                if (m != null)
                {
                    menus.Add(m);
                }
            }
            return menus.OrderBy(b=>b.SequenceCode).ToList() ;
        }
        static Dictionary<string, List<Menu>> userMenuCache = new Dictionary<string, List<Menu>>();
        /// <summary>
        /// 移除用户菜单缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="systemTypeId"></param>
        public void RemoveUserMenuCache(int userId, int systemTypeId)
        {
            string cacheKey = string.Format("menu_{0}_{1}", userId, systemTypeId);
            userMenuCache.Remove(cacheKey);
        }
        public void RemoveUserMenuCache()
        {
            userMenuCache.Clear();
        }
        /// <summary>
        /// 获取用户菜单
        /// 如果显示导航菜单,请判断ShowInNav
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="systemTypeId"></param>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        public List<Menu> GetUserMenu(int userId, int systemTypeId, bool fromCache = true)
        {
            string cacheKey = string.Format("menu_{0}_{1}", userId, systemTypeId);
            if (fromCache)
            {
                if (userMenuCache.ContainsKey(cacheKey))
                {
                    return userMenuCache[cacheKey];
                }
            }
            var u = EmployeeBusiness.Instance.QueryItem(b => b.Id == userId);
            if (u == null)
            {
                throw new Exception("获取菜单出错,找不到指定的用户:" + userId);
            }
            var controls = AccessControlBusiness.Instance.QueryList(b => ((b.Role == userId && b.RoleType == RoleType.用户) || (b.Role == u.Role && b.RoleType == RoleType.角色)) && b.SystemTypeId == systemTypeId && b.Que == true);
            List<Menu> list = new List<Menu>();
            var allCache = GetAllCache(systemTypeId);
            //list.AddRange(allCache.FindAll(b => b.SequenceCode.Length == 2));
            foreach (var item in controls)
            {
                if (list.Find(b => b.SequenceCode==item.MenuCode) != null)
                {
                    continue;
                }
                var m = Get(item.MenuCode, systemTypeId);
                if (m == null)
                    continue;
                if (!m.Disable)
                {
                    list.Add(m);
                    var parent = list.Find(b => b.SequenceCode == m.ParentCode);
                    if (parent == null)
                    {
                        list.Add(allCache.Where(b => b.SequenceCode == m.ParentCode).First());
                    }
                }
            }

            var list2 = list.OrderBy(b => b.SequenceCode).ToList();
            if (!userMenuCache.ContainsKey(cacheKey))
            {
                userMenuCache.Add(cacheKey, list2);
            }
            return list2;
        }

        /// <summary>
        /// 分组显示用户菜单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="systemTypeId"></param>
        /// <returns></returns>
        public Dictionary<Menu, List<Menu>> GetUserMenuByGroup(int userId, int systemTypeId)
        {
            var menus = GetUserMenu(userId, systemTypeId);
            var dic = new Dictionary<Menu, List<Menu>>();
            Menu lastMenu = null;
            var menu1 = menus.FindAll(b=>b.SequenceCode.Length==2);
            foreach (var item in menu1)
            {
                dic.Add(item, new List<Menu>());
            }
            var menu2 = menus.FindAll(b => b.SequenceCode.Length >2);
            foreach (var item in menu2)
            {
                var m = menu1.Find(b => b.SequenceCode == item.SequenceCode.Substring(0, 2));
                if (m != null)
                {
                    dic[m].Add(item);
                }
            } 
            //foreach (var item in menus)
            //{
            //    if (item.SequenceCode.Length == 2)
            //    {
            //        lastMenu = item;
            //        dic.Add(lastMenu, new List<CRL.RoleAuthorize.Menu>());
            //    }
            //    else
            //    {
            //        if (lastMenu == null)//数据错误导致找不到之前菜单
            //        {
            //            continue;
            //        }
            //        dic[lastMenu].Add(item);
            //    }
            //}
            dic = dic.OrderByDescending(b => b.Key.Sort).ToDictionary(b => b.Key, c => c.Value.OrderByDescending(d => d.Sort).ToList());
            return dic;
        }
        public List<Menu> GetFavMenus(int systemTypeId, int userId, int num = 5)
        {
            Dictionary<string, int> dic = GetFavMenuDic(systemTypeId, userId);
            List<Menu> menus = new List<Menu>();
            foreach (var item in dic.Keys)
            {
                var menu = GetAllCache(systemTypeId).Where(b => b.SequenceCode == item).FirstOrDefault();
                if (menu == null)
                    continue;
                if (!menu.ShowInNav)
                {
                    continue;
                }
                menu.Hit = dic[item];
                menus.Add(menu);
            }
            return menus.OrderByDescending(b => b.Hit).Take(num).ToList();
        }
        public void SaveFavMenus(Dictionary<string, int> dic, int systemTypeId, int userId)
        {
            string name = string.Format("{0}_{1}.txt", systemTypeId, userId);
            string folder = System.Web.HttpContext.Current.Server.MapPath("/config/userMenuCache/");
            CoreHelper.EventLog.CreateFolder(folder);
            string fileName = folder + name;
            string str = "";
            foreach (var key in dic.Keys)
            {
                str += string.Format("{0}:{1};", key, dic[key]);
            }
            System.IO.File.WriteAllText(fileName, str);
        }
        public Dictionary<string, int> GetFavMenuDic(int systemTypeId, int userId)
        {
            string name = string.Format("{0}_{1}.txt", systemTypeId, userId);
            string folder = System.Web.HttpContext.Current.Server.MapPath("/config/userMenuCache/");
            string fileName = folder + name;
            string menus = "";
            if (System.IO.File.Exists(fileName))
            {
                menus = System.IO.File.ReadAllText(fileName);
            }
            string[] arry = menus.Split(';');
            Dictionary<string, int> dic = new Dictionary<string, int>();
            foreach (string s in arry)
            {
                if (s.Trim() == "")
                    continue;
                string[] arry2 = s.Split(':');
                if (arry2.Length < 2)
                    continue;
                string menuCode = arry2[0];
                int hit = Convert.ToInt32(arry2[1]);
                if (!dic.ContainsKey(menuCode))
                {
                    dic.Add(menuCode, hit);
                }
            }
            return dic;
        }
    }
}
