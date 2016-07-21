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
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var item = Code.ProductDataManage.Instance.QueryItemFromCache(b => b.Id > 0 && b.ProductName.Contains("product"));
            sw.Stop();

            Response.Write(string.Format("Compile：{0} value:{1}", sw.ElapsedMilliseconds, item.Number));
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var item = Code.ProductDataManage.Instance.QueryItem(b => b.Id > 0);
            item.Number = DateTime.Now.Second;
            Code.ProductDataManage.Instance.Update(item);
            //CRL.CacheServerSetting.CacheClientProxy.GetServerTypeSetting();
        }
    }
}
