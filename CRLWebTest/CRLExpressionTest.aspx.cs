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
    public partial class CRLExpressionTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //将lambda表达式转换可传输的json,并还原

            int m = 10;
            var query = new CRL.LambdaQuery.CRLExpression.CRLExpressionVisitor<Code.ProductData>();
            var re = query.Where(b => b.Id.Equals(2));
            //var re = query.Where(b => b.Id==2);
            //var re = query.Where(b => (b.Id | 1) == 1);
            //var re = query.Where(b => int.Parse(b.CategoryName) == 11);
            //var re = query.Where(b => b.CategoryName.ToUpper()=="22");
            //var re = query.Where(b => b.Id.ToString()=="22");
            //var re = query.Where(b => Convert.ToInt32(b.Style) == m);
            //var re = query.Where(b => b.ProductName.Substring(2)=="22");
            //var re = query.Where(b => b.ProductName.IndexOf("22")==1);
            //var re = query.Where(b => b.ProductName.IndexOf("22") == 1&&b.Id==m);
            //还原
            var obj = CRL.LambdaQuery.CRLExpression.CRLQueryExpression.FromJson(re);
            var expression = query.CreateLambda(obj.Exp);
            var pc = expression.Compile();
            //re = CRL.CacheServer.CacheServer.Query(obj);
            TextBox1.Text = re.ToString();
        }
    }
}
