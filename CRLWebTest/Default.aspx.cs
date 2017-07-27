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
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            //query.UseInJoin(6);
            query.Where(b => b.ProductChannel.In(new ProductChannel[] { ProductChannel.其它, ProductChannel.自采 }));
            //query.Where(b => b.CategoryName.In("12", "212", "122", "2424", "122", "2424", "122"));
            //query.Where(b => b.CategoryName.Replace("22","") == "abc");
            //var result = query.ToList();
            Response.Write(query.PrintQuery());
            //Response.End();
        }
        
    }
}
