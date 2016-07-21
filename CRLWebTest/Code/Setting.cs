/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics; 
namespace WebTest.Code
{
    public class Setting
    {
        public static string GetVersion()
        {
            return CRL.Base.GetVersion().ToString();
            string path = HttpContext.Current.Server.MapPath("/bin/CRL.dll");
            FileVersionInfo myFileVersion = FileVersionInfo.GetVersionInfo(path);
            return myFileVersion.FileVersion; 
        
        }
        public static string Value1 = GetVersion();
    }
}
