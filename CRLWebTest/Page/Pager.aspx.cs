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
    public partial class Pager : System.Web.UI.Page
    {
        class ClassTemp
        {
            public string ProductName { get; set; }
            public string BarCode { get; set; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //多表关联查询
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Where(b => b.Id > 0);
            query.Join<Code.Member>((a, b) => a.UserId == b.Id).Select((a, b) => new { a.ProductName, b.Name });//筛选返回的字段
            query.Join<Code.Order>((a, b) => a.UserId == b.UserId).Select((a, b) => new { orderid = b.OrderId });//筛选返回的字段
            query.OrderBy(b=>b.BarCode);
            query.Page(15,1);//如果要分页,设定分页参数就行了
            //var list = query.ToDynamic();
            int total = query.RowCount;
            var sql = query.PrintQuery();
        }


        protected void Button3_Click(object sender, EventArgs e)
        {
            int pageSize = 15;
            int page = 1;
            int count;
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Where(b => b.Id > 0);
            query.Page(pageSize, page);
            query.OrderBy(b => b.Id, true);
            var list = query.ToList();//返回当前对象类型
            count = query.RowCount;
            txtOutput.Visible = true;
            txtOutput.Text = query.PrintQuery();
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            //using CRL以获取扩展方法
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Page(15, 1);
            query.Where(b => b.Id>0);
            int count;
            //选择GROUP字段
            query.Select(b => new
            {
                b.BarCode,
                b.ProductName,
                total = b.BarCode.COUNT(),//等效为count(BarCode) as total
                sum1 = b.Number.SUM()//等效为sum(Number) as sum1
            });
            //GROUP条件
            query.GroupBy(b => new { b.BarCode, b.ProductName });
            //having
            query.GroupHaving(b => b.Number.SUM() >= 0);
            //设置排序
            query.OrderBy(b => b.BarCode.Count(), true);//等效为 order by count(BarCode) desc
            var list = query.ToDynamic();//按选择的字段返回动态类型
            txtOutput.Visible = true;
            txtOutput.Text = query.PrintQuery();
            foreach (dynamic item in list)
            {
                var str = string.Format("{0}______{1} {2} {3}<br>", item.BarCode, item.ProductName, item.total, item.sum1);//动态对象
                Response.Write(str);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            int pageSize = 4;
            int page = 1;
            int count;
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Select(b => new { b.ProductName, b.BarCode });//选择指定字段
            query.Where(b => b.Id > 0);
            query.Page(pageSize, page);
            query.OrderBy(b => b.Id, true);
            var list = query.ToList<ClassTemp>();//按选择字段指定类型转换
            count = query.RowCount;
            txtOutput.Visible = true;
            txtOutput.Text = query.PrintQuery();
        }
    }
}
