/**
* CRL 快速开发框架 V3.1
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
    public partial class Delete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Code.ProductDataManage.Instance.Delete(b => b.Id == 0);//按条件删除
            Code.ProductDataManage.Instance.Delete(1);//按主键删除
            Code.OrderManage.Instance.Delete<Code.ProductData>((a, b) => a.UserId == b.Id);//关联ProductData删除
        }

        protected void Button1_Click1(object sender, EventArgs e)
        {

        }
    }
}
