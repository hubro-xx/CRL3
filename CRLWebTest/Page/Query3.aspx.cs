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
using CRL;
namespace WebTest.Page
{
    public partial class Query3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var count = Code.MemberManage.Instance.Count(b => b.Id > 0);
            //if (count == 0)
            //{
            //    var m = new Code.Member() { Name = "hubro" };
            //    int n = Code.MemberManage.Instance.Add(m);
            //    var c = new CRL.ParameCollection();
            //    c["UserId"] = n;
            //    Code.ProductDataManage.Instance.Update(b => b.Id > 0, c);
            //}
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //返回筛选值
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Top(10);
            var member = new Code.Member();
            member.Id = 11;
            query.Join<Code.Member>((a, b) => a.UserId == member.Id && b.Id > 0,
                 CRL.LambdaQuery.JoinType.Left
                ).Select((a, b) => new { BarCode1 = a.BarCode, Name1 = b.Name });
            var list = query.ToDynamic();
            txtOutput.Visible = true;
            txtOutput.Text = query.PrintQuery();
            foreach (dynamic item in list)
            {
                var str = string.Format("{0}______{1}<br>", item.BarCode1, item.Name1);//动态对象
                Response.Write(str);
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            //把关联值存入对象内部索引
            //关联对象值都以索引方式存取
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Top(10);
            query.Join<Code.Member>((a, b) => a.UserId == b.Id && b.Id > 0,
                CRL.LambdaQuery.JoinType.Left
                ).SelectAppendValue(b => new { Name1 = b.Name });
            var list = query.ToList();
            txtOutput.Visible = true;
            txtOutput.Text = query.PrintQuery();
            foreach (var item in list)
            {
                var str = string.Format("{0}______{1}<br>", item.BarCode, item.Bag.Name1);//取名称为Name1的索引值
                Response.Write(str);
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Top(10);
            query.Where(b => b.Id > 0);
            query.Join<Code.Member>((a, b) => a.UserId == b.Id && b.Id > 0,
                CRL.LambdaQuery.JoinType.Left
                );
            //todo 不能多个对象类型返回匿名类
            var sql = query.PrintQuery();
            var list = query.ToDynamic(b => new { b.BarCode, Name = b.ProductName });
            foreach (var item in list)
            {
                var str = string.Format("{0}______{1}<br>", item.BarCode, item.Name);//动态对象
                Response.Write(str);
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Where(b => b.ProductId == "0");
            //等效为 product.UserId in(select UserId from order where product.SupplierId=10 and order.status=2)
            query.In<Code.Order>(b => b.UserId, b => b.UserId, (a, b) => a.SupplierId == "10" && b.Status == 2);
            txtOutput.Visible = true;
            txtOutput.Text = query.PrintQuery();
        }
    }
}
