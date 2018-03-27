/**
* CRL 快速开发框架 V4.5
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest.Page
{
    public partial class DbSet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var order = new Code.Order();
            //所有
            var product = order.Products.ToList();

            //返回关联过的查询,使用完整查询满足更多需求
            var product2 = order.Products.GetQuery();

            var p = new Code.ProductData() { BarCode = "33333" };
            //添加一项
            order.Products.Add(p);

            order.Products.Remove(p);//删除一项
            order.Products.Save();
            //返回完整的BaseProvider
            var provider = order.Products.GetProvider();

            //返回关联的member,在调用时返回,在循环内调用会多次调用数据库
            var member = order.Member.Value;
        }
    }
}
