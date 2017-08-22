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
using CRL;
using WebTest.Code;
namespace WebTest
{
    public partial class Default : System.Web.UI.Page
    {
        int id = 20;
        protected void Page_Load(object sender, EventArgs e)
        {
            //Code.ProductDataManage.Instance.Test2();
            var query3 = Code.ProductDataManage.Instance.GetLambdaQuery();
            query3.Join<Code.Member>((a, b) => a.Id == b.Id).SelectAppendValue(b => b.Name);
            query3.Where(b => b.Id > 1);
            var resutl3 = query3.ToList();
            foreach (var d in resutl3)
            {
                var a = d.Bag.Name;
            }
            //var n2 = GC.GetTotalMemory(true);
            //var list1 = Code.ProductDataManage.Instance.GetLambdaQuery().Top(1000).ToList();
            //var list2 = Code.OrderManage.Instance.GetLambdaQuery().Top(1000).ToList();
            //var n3 = GC.GetTotalMemory(false) - n2;
            //Response.Write("ok " + n3 / 1024);
            Response.End();
        }
    }
}
