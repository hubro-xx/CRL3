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

        public static Code.ProductData data2
        {
            get
            {
                return new Code.ProductData() { Id = 10 };
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Code.TestAll.TestSelect();
        }
    }
}
