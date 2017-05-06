/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest.Page
{
    public partial class Cache2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            int n = 10;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                var item = Code.CacheDataTestManage.Instance.QueryItemFromCache(b => b.Id > 0 && b.Name.Contains("name"));
            }
            sw.Stop();

            Response.Write(string.Format("查询" + n + "次,用时：{0} ", sw.ElapsedMilliseconds));
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var item = Code.CacheDataTestManage.Instance.QueryItem(b => b.Id > 0);
            item.Name = DateTime.Now.Second.ToString();
            Code.CacheDataTestManage.Instance.Update(item);
            //CRL.CacheServerSetting.CacheClientProxy.GetServerTypeSetting();
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                var index = i;
                System.Threading.Tasks.Task.Run(()=>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    try
                    {
                        var item = Code.CacheDataTestManage.Instance.QueryItemFromCache(b => b.Id > 0 && b.Name.Contains("name"));
                        sw.Stop();
                        CoreHelper.EventLog.Log("用时" + sw.ElapsedMilliseconds + " 在线程" + index, "cache");
                    }
                    catch(Exception ero)
                    {
                        CoreHelper.EventLog.Log("错误在线程" + index + " " + ero.Message, "cache");
                    }

                });
            }
        }
    }
}
