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

namespace RoleControl
{
    public class ConnectionManage
    {
        static string file = "/config/LocalConnection.config";
        public static List<string> cache = new List<string>();
        public static int index = 0;
        public static List<string> GetConnections()
        {
            if (cache.Count == 0)
            {
                string file1 = System.Web.Hosting.HostingEnvironment.MapPath(file);
                if (!System.IO.File.Exists(file1))
                {
                    throw new Exception("请配置:" + file);
                }
                cache = System.IO.File.ReadAllLines(file1).ToList();
            }
            return cache;
        }
        public static string GetCurrent()
        {
            var list = GetConnections();
            return list[index];
        }
        public static void Save()
        {
            string file1 = System.Web.Hosting.HostingEnvironment.MapPath(file);
            System.IO.File.WriteAllLines(file1, cache.ToArray());
        }
    }
}
