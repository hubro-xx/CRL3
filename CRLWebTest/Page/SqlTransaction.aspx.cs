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

namespace WebTest
{
    public partial class SqlTransaction : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //protected void Button1_Click(object sender, EventArgs e)
        //{
        //    string message;
        //    bool a = Code.ProductDataManage.Instance.TransactionTest(out message);
        //    Response.Write("操作" + a + message);
        //}

        //protected void Button2_Click(object sender, EventArgs e)
        //{
        //    Code.OrderManage.Instance.TransactionTest2();
        //}

        protected void Button3_Click(object sender, EventArgs e)
        {
            string error;
            var watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 10; i++)
            {
                var a = Code.OrderManage.Instance.TransactionTest(out error);
            }
            watch.Stop();
            Response.Write("操作" + watch.ElapsedMilliseconds);
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            string error;
            var watch = new Stopwatch();
            watch.Start();
            var a = Code.OrderManage.Instance.TransactionTest2(out error);
            watch.Stop();
            Response.Write("提交" + a + error);
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            var item = Code.ProductDataManage.Instance.QueryItem(b => b.Id > 0);
            string error;
            var a = Code.OrderManage.Instance.TransactionTest3(out error);
            var allDBContext = CRL.Base.GetCallDBContext();
            Response.Write("提交" + a + error);
        }
    }
}
