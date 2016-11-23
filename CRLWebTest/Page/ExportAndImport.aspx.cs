/**
* CRL 快速开发框架 V4.0
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

namespace WebTest
{
    public partial class ExportAndImport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //导出为JSON
            var json = Code.ProductDataManage.Instance.ExportToJson(b => b.Id > 0);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Code.ProductDataManage.Instance.ImportFromJson("json串", b => b.Id > 0);
        }
    }
}
