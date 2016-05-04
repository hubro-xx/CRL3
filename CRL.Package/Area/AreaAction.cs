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

namespace CRL.Package.Area
{
    /// <summary>
    /// 查询地区区域
    /// 数据不依赖数据库,从资源中加载Resources.area
    /// </summary>
    public class AreaAction 
    {
        static Dictionary<string, Area> cache = new Dictionary<string, Area>();

        public static Dictionary<string, Area> Cache
        {
            get
            {
                if (cache.Count == 0)
                {
                    #region 读取
                    //source select ID,Name,OwnerID,LevelID from area
                    string area = Properties.Resources.area;
                    string[] arry = area.Split('\r');
                    foreach (string s in arry)
                    {
                        string item = s.Trim();
                        if (string.IsNullOrEmpty(item))
                            continue;
                        item = System.Text.RegularExpressions.Regex.Replace(item, @"\s+", ",");
                        string[] arry1 = item.Split(',');
                        if (arry1.Length == 0)
                            continue;
                        Area a = new Area();
                        a.Code = arry1[0];
                        a.Name = arry1[1];
                        a.ParentCode = arry1[2];
                        if (a.ParentCode == "1")
                        {
                            a.Level = 1;
                        }
                        else if (a.Code.Substring(3, 3) == "100")
                        {
                            a.Level = 2;
                        }
                        else
                        {
                            a.Level = 3;
                        }
                        cache.Add(a.Code, a);
                    }
                    #endregion
                }
                return AreaAction.cache;
            }
        }
        /// <summary>
        /// 根据父级获取所有子级
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public static List<Area> GetChild(string parentCode)
        {
            List<Area> list = new List<Area>();
            foreach (Area a in Cache.Values)
            {
                if (a.ParentCode == parentCode)
                {
                    list.Add(a);
                }
            }
            return list;
        }
        public static Area Get(string code)
        {
            if (Cache.ContainsKey(code))
                return Cache[code];
            return null;
        }
        /// <summary>
        /// 返回全称
        /// </summary>
        /// <param name="county"></param>
        /// <param name="pad"></param>
        /// <returns></returns>
        public static string GetFullName(string county,string pad=" ")
        {
            string str = "";
            if (string.IsNullOrEmpty(county))
            {
                return str;
            }
            str += Get(county.Substring(0, 2) + "0000") + pad;
            str += Get(county.Substring(0, 4) + "00") + pad;
            str += Get(county);
            return str;
        }
    }
}
