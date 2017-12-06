/**
* CRL 快速开发框架 V4.5
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
    public partial class CallContext : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();
            test1();
            sw.Stop();
            Response.Write(sw.ElapsedMilliseconds + "<br>");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Restart();
            test2();
            sw.Stop();
            Response.Write(sw.ElapsedMilliseconds + "<br>");
        }
        void test1()
        {
            for (int i = 0; i < 100; i++)
            {
                Code.OrderManage.Instance.QueryItem(i);
            }
        }
        void test2()
        {
            using (var context = new CRL.CRLDbConnectionScope())
            {
                for (int i = 0; i < 100; i++)
                {
                    Code.OrderManage.Instance.QueryItem(i);
                }
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Code.OrderManage.Instance.QueryItem(b => b.Id == 3);
            var item2 = Code.OrderManage.Instance.QueryItem(b => b.Id == 2);
            Code.ProductDataManage.Instance.QueryItem(2);
            var keys = CRL.Base.GetCallDBContext();
            Response.Write("调用数据访问对象为" + string.Join(",", keys));
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            Code.OrderManage.Instance.QueryItem(b => b.Id == 3);
            var item2 = Code.OrderManage.Instance.QueryItem(b => b.Id == 2);
            Code.ProductDataManage.Instance.QueryItem(2);
            bool a;
            var allCall = CRL.Base.GetSQLRunningtime(out a);
            GridView1.DataSource = allCall;
            GridView1.DataBind();
        }
    }
}
