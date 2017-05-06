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
    public partial class Query1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var a = true;
            var list = new List<object>();
            //使用同一个数据连接
            Code.ProductDataManage.Instance.PackageMethod(() =>
            {
                var item = Code.ProductDataManage.Instance.QueryItem(2);
                var item2 = Code.ProductDataManage.Instance.QueryItem(2);
            });
            //查询一项
            using (var context = new CRL.CRLDbConnectionScope())//使用同一个数据连接
            {
                var item = Code.ProductDataManage.Instance.QueryItem(b => b.Id > 0 || b.IsTop == a);
                var item2 = Code.ProductDataManage.Instance.QueryItem(2);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            GridView1.DataSource = list;
            GridView1.DataBind();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            //查询集合
            var list = Code.ProductDataManage.Instance.QueryList(b => b.Id > 0 && b.Id < 200);
            GridView1.DataSource = list;
            GridView1.DataBind();
        }
    }
}
