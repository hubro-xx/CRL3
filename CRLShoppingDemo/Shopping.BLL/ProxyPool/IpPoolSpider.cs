using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shopping.BLL.ProxyPool
{
    /// <summary>
    /// IP池 抓取蜘蛛
    /// TODO:代理池站点变化较快，时常关注日志监控
    /// </summary>
    public class IpPoolSpider
    {
        public void Initial()
        {
            Downloadproxy360(null);
            DownloadproxyBiGe(null);
            Downloadproxy66(null);
            Downloadxicidaili(null);
            //ThreadPool.QueueUserWorkItem(Downloadproxy360);
            //ThreadPool.QueueUserWorkItem(DownloadproxyBiGe);
            //ThreadPool.QueueUserWorkItem(Downloadproxy66);
            //ThreadPool.QueueUserWorkItem(Downloadxicidaili);
        }

        // 下载西刺代理的html页面
        public void Downloadxicidaili(object DATA)
        {
            try
            {
                List<string> list = new List<string>()
                {
                    "http://www.xicidaili.com/nt/",
                    "http://www.xicidaili.com/nn/",
                    "http://www.xicidaili.com/wn/",
                    "http://www.xicidaili.com/wt/"

                };
                foreach (var utlitem in list)
                {
                    for (int i = 1; i < 5; i++)
                    {
                        string url = utlitem + i.ToString();
                        //var ipProxy = PoolManageService.GetProxy();
                        //if (string.IsNullOrEmpty(ipProxy))
                        //{
                        //    LogUtils.ErrorLog(new Exception("Ip代理池暂无可用代理IP"));
                        //    return;
                        //}
                        //var ip = ipProxy;
                        //WebProxy webproxy;
                        //if (ipProxy.Contains(":"))
                        //{
                        //    ip = ipProxy.Split(new[] { ':' })[0];
                        //    var port = int.Parse(ipProxy.Split(new[] { ':' })[1]);
                        //    webproxy = new WebProxy(ip, port);
                        //}
                        //else
                        //{
                        //    webproxy = new WebProxy(ip);
                        //}
                        string html = HttpHelper.DownloadHtml(url, null);
                        if (string.IsNullOrEmpty(html))
                        {
                            LogUtils.ErrorLog(new Exception("代理地址：" + url + " 访问失败"));
                            continue;
                        }

                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(html);
                        HtmlNode node = doc.DocumentNode;
                        string xpathstring = "//tr[@class='odd']";
                        HtmlNodeCollection collection = node.SelectNodes(xpathstring);
                        foreach (var item in collection)
                        {
                            var proxy = new IpProxy();
                            string xpath = "td[2]";
                            proxy.Address = item.SelectSingleNode(xpath).InnerHtml;
                            xpath = "td[3]";
                            proxy.Port = int.Parse(item.SelectSingleNode(xpath).InnerHtml);
                            Task.Run(() =>
                            {
                                PoolManageService.Add(proxy);
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtils.ErrorLog(new Exception("下载西刺代理IP池出现故障", e));
            }
        }

        // 下载快代理
        public void Downkuaidaili(object DATA)
        {
            try
            {
                string url = "http://www.kuaidaili.com/proxylist/";
                for (int i = 1; i < 4; i++)
                {
                    string html = HttpHelper.DownloadHtml(url + i.ToString() + "/", null);
                    string xpath = "//tbody/tr";
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    HtmlNode node = doc.DocumentNode;
                    HtmlNodeCollection collection = node.SelectNodes(xpath);
                    foreach (var item in collection)
                    {
                        var proxy = new IpProxy();
                        proxy.Address = item.FirstChild.InnerHtml;
                        xpath = "td[2]";
                        proxy.Port = int.Parse(item.SelectSingleNode(xpath).InnerHtml);
                        Task.Run(() =>
                        {
                            PoolManageService.Add(proxy);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                LogUtils.ErrorLog(new Exception("下载快代理IP池出现故障", e));
            }
        }

        // 下载proxy360
        public void Downloadproxy360(object DATA)
        {
            try
            {
                string url = "http://www.proxy360.cn/default.aspx";
                string html = HttpHelper.DownloadHtml(url, null);
                if (string.IsNullOrEmpty(html))
                {
                    LogUtils.ErrorLog(new Exception("代理地址：" + url + " 访问失败"));
                    return;
                }
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                string xpathstring = "//div[@class='proxylistitem']";
                HtmlNode node = doc.DocumentNode;
                HtmlNodeCollection collection = node.SelectNodes(xpathstring);

                foreach (var item in collection)
                {
                    var proxy = new IpProxy();
                    var childnode = item.ChildNodes[1];
                    xpathstring = "span[1]";
                    proxy.Address = childnode.SelectSingleNode(xpathstring).InnerHtml.Trim();
                    xpathstring = "span[2]";
                    proxy.Port = int.Parse(childnode.SelectSingleNode(xpathstring).InnerHtml);
                    Task.Run(() =>
                    {
                        PoolManageService.Add(proxy);
                    });
                }
            }
            catch (Exception e)
            {
                LogUtils.ErrorLog(new Exception("下载proxy360IP池出现故障", e));
            }
        }

        // 下载逼格代理
        public void DownloadproxyBiGe(object DATA)
        {
            try
            {
                List<string> list = new List<string>()
                {
                    "http://www.bigdaili.com/dailiip/1/{0}.html",
                    "http://www.bigdaili.com/dailiip/2/{0}.html",
                    "http://www.bigdaili.com/dailiip/3/{0}.html",
                    "http://www.bigdaili.com/dailiip/4/{0}.html"
                };
                foreach (var utlitem in list)
                {
                    for (int i = 1; i < 5; i++)
                    {
                        string url = String.Format(utlitem, i);
                        string html = HttpHelper.DownloadHtml(url, null);
                        if (string.IsNullOrEmpty(html))
                        {
                            LogUtils.ErrorLog(new Exception("代理地址：" + url + " 访问失败"));
                            continue;
                        }

                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(html);
                        HtmlNode node = doc.DocumentNode;
                        string xpathstring = "//tbody/tr";
                        HtmlNodeCollection collection = node.SelectNodes(xpathstring);
                        foreach (var item in collection)
                        {
                            var proxy = new IpProxy();
                            var xpath = "td[1]";
                            proxy.Address = item.SelectSingleNode(xpath).InnerHtml;
                            xpath = "td[2]";
                            proxy.Port = int.Parse(item.SelectSingleNode(xpath).InnerHtml);
                            Task.Run(() =>
                            {
                                PoolManageService.Add(proxy);
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtils.ErrorLog(new Exception("下载逼格代理IP池出现故障", e));
            }
        }

        // 下载66免费代理
        public void Downloadproxy66(object DATA)
        {
            try
            {
                List<string> list = new List<string>()
                {
                    "http://www.66ip.cn/areaindex_35/index.html",
                    "http://www.66ip.cn/areaindex_35/2.html",
                    "http://www.66ip.cn/areaindex_35/3.html"
                };
                foreach (var utlitem in list)
                {
                    string url = utlitem;
                    string html = HttpHelper.DownloadHtml(url, null);
                    if (string.IsNullOrEmpty(html))
                    {
                        LogUtils.ErrorLog(new Exception("代理地址：" + url + " 访问失败"));
                        break;
                    }

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    HtmlNode node = doc.DocumentNode;
                    string xpathstring = "//table[@bordercolor='#6699ff']/tr";
                    HtmlNodeCollection collection = node.SelectNodes(xpathstring);
                    foreach (var item in collection)
                    {
                        var proxy = new IpProxy();
                        var xpath = "td[1]";
                        proxy.Address = item.SelectSingleNode(xpath).InnerHtml;
                        if (proxy.Address.Contains("ip"))
                        {
                            continue;
                        }
                        xpath = "td[2]";
                        proxy.Port = int.Parse(item.SelectSingleNode(xpath).InnerHtml);
                        Task.Run(() =>
                        {
                            PoolManageService.Add(proxy);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                LogUtils.ErrorLog(new Exception("下载66免费代理IP池出现故障", e));
            }
        }
    }
}
