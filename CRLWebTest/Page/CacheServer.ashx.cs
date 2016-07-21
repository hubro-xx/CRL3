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

namespace WebTest.Page
{
    /// <summary>
    /// CacheServer 的摘要说明
    /// </summary>
    public class CacheServer : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string q = context.Request["q"];

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
