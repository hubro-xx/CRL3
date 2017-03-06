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
            var name = Request["name"];
            var manage = Code.ProductDataManage.Instance;
            var query = manage.GetLambdaQuery();
            query.Where(b => b.CategoryName.Like(name));
            Response.Write(query.PrintQuery());
            //Code.TestAll.TestUpdate();
            //return;
            //Update.TestModified();
        }
    }
}
