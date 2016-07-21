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
    public partial class Insert : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var count = Code.ProductDataManage.Instance.Count(b => b.Id > 0);
            //Response.Write(count);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var item = new Code.ProductData() { InterFaceUser = "2222", ProductName = "product2", BarCode = "1212122",UserId=1,Number=10 };

            Code.ProductDataManage.Instance.Add(item);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var list = new List<Code.ProductData>();
            for (int i = 1; i < 10; i++)
            {
                list.Add(new Code.ProductData() { InterFaceUser = "2222", ProductName = "product" + i, BarCode = "code" + i,UserId=1,Number=i });
            }
            Code.ProductDataManage.Instance.BatchInsert(list);
        }
    }
}
