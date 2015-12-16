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