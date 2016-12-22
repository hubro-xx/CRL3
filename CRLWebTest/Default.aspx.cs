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
            var query = ProductDataManage.Instance.GetLambdaQuery();
            query.Take(10);
            int n = 20;
            string name = "sss";
            //var join = query.Join<Code.Member>((a, b) => a.UserId == b.Id)
            //    .SelectAppendValue(b => b.Mobile).OrderBy(b => b.Id, true);
            //query.Where(b => b.Id > n && b.CategoryName.Contains(name));
            query.Where(b => b.IsTop || b.CategoryName==null);
            //join.Where(b => b.AccountNo == "123");//按join追加条件
            string sql = query.PrintQuery();
            var resut = query.SelectV(b => new {b.CategoryName,b.Id }).ToList();
            Response.Write(sql);
            Response.End();
        }
    }
}
