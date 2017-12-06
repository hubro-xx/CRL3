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
using CRL;
using WebTest.Code;
namespace WebTest
{
    public partial class Default : System.Web.UI.Page
    {
        int id = 20;
        protected void Page_Load(object sender, EventArgs e)
        {
            var order = new Code.Order();
            //var result = order.Products.Where(b => b.Id > 0).ToList();

            var resutl2 = ProductDataManage.Instance.QueryFromCache(b => b.ProductName.Len() > 2);

            Code.ProductDataManage.Instance.DynamicQueryTest();
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Join<Code.Member>((a, b) => a.Id == b.Id).Select((a, b) => new
            {
                aa1 = b.Id,
                ss2 = a.TransType
            });
            query.Where(b=>b.Id==2);
            var sql = query.ToString();
            //var n2 = GC.GetTotalMemory(true);
            //var list1 = Code.ProductDataManage.Instance.GetLambdaQuery().Top(1000).ToList();
            //var list2 = Code.OrderManage.Instance.GetLambdaQuery().Top(1000).ToList();
            //var n3 = GC.GetTotalMemory(false) - n2;
            Response.Write(sql);
            Response.End();
        }
        class testA
        {
            public string CategoryName
            {
                get;set;
            }
            public string ProductName
            {
                get;set;
            }
        }
    }
}
