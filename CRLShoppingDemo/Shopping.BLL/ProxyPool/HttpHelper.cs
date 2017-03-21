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