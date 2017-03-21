using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace Shopping.BLL.ProxyPool
{
    public class PoolManageService
    {
        internal static void Add(IpProxy proxy)
        {
            proxy.FullAddress = string.Format("{0}:{1}", proxy.Address, proxy.Port);
            var count = IpProxyManage.Instance.Count(b => b.FullAddress == proxy.FullAddress);
            if (count == 0)
            {
                IpProxyManage.Instance.Add(proxy);
            }
        }
        public static bool Test(IpProxy proxy,out string error)
        {
            error = "";
            try
            {
                //var ms = HttpHelper.DownloadHtml("http://news.163.com/", proxy.FullAddress);
                var ip = IPAddress.Parse(proxy.Address);
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.SendTimeout = 500;
                sock.Connect(ip, proxy.Port);
                sock.Close();
                proxy.CheckResult = true;
            }
            catch (Exception ero)
            {
                error = ero.Message;
                proxy.CheckResult = false;
            }
            proxy.CheckTime = DateTime.Now;
            IpProxyManage.Instance.Update(proxy);
            return proxy.CheckResult;
        }
    }
}