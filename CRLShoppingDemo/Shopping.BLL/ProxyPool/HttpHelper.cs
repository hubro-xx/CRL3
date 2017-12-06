/**
* CRL 快速开发框架 V4.5
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Shopping.BLL.ProxyPool
{
    public class HttpHelper
    {
        internal static string DownloadHtml(string url, string webproxy)
        {
            var myStream = CoreHelper.HttpRequest.HttpGet(url, webproxy);
            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.Default);
            var strResult = sr.ReadToEnd();
            sr.Close();
            myStream.Close();
            return strResult;
        }
    }
}
