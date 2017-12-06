/**
* CRL 快速开发框架 V4.5
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.BLL.ProxyPool
{
    public class LogUtils
    {
        public static void ErrorLog(Exception ero)
        {
            CoreHelper.EventLog.Log(ero.ToString(), "", false);
        }
    }
}
