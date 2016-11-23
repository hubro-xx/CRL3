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
namespace WebTest
{
    public partial class Query2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        class classA
        {
            public string Name;
            public string Method()
            {
                return "ffffff" + DateTime.Now.Second;
            }
        }
        public enum Status
        {
            正常,
            不正常
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Select(b => new { b.InterFaceUser, b.ProductName, b.PurchasePrice });//选择查询的字段
            int? n2 = 10;
            classA a = new classA() { Name = "ffffff" };
            query.Top(100);//取多少条
            query.Where(b => b.Id < 700);//查询条件
            query.Where(b => b.ProductName.Contains("w2") || b.ProductName.Contains("sss"));
            string s = "ssss";
            int n = 10;

            query.Where(b => b.ProductName == s && b.Id > n || b.ProductName.Contains("sss"));
            query.Where(b => b.Id == n2.Value);
            query.Where(b => b.ProductName == a.Name);
            query.Where(b => b.ProductName == a.Method());
            query.Where(b => b.ProductName.Contains("sss"));
            query.OrderBy(b => b.Id, true);//排序条件
            query.OrderBy(b => b.Number, false);//多列排序调用多次
            txtOutput.Visible = true;
            txtOutput.Text = query.PrintQuery();

            var list = query.ToList();
            Response.Write(string.Format("解析语法用时:{0}ms 数据查询用时:{1}ms 对象映射用时:{2}ms {3}行", query.AnalyticalTime, query.ExecuteTime, query.MapingTime, list.Count));
        }



        protected void Button5_Click(object sender, EventArgs e)
        {
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Select(b => new { b.ProductChannel, b.BarCode });
            query.Where(b=>b.Id<10);
            var list = query.ToDynamic();
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            txtOutput.Visible = false;
            //SUM
            //按条件id>0,合计Number列
            var sum = Code.ProductDataManage.Instance.Sum(b => b.Id > 0, b => b.Number * b.UserId);
            //按条件id>0,进行总计
            var count = Code.ProductDataManage.Instance.Count(b => b.Id > 0);
            var max = Code.ProductDataManage.Instance.Max(b => b.Id > 0, b => b.Id);
            var min = Code.ProductDataManage.Instance.Min(b => b.Id > 0, b => b.Id);
        }

        protected void Button7_Click(object sender, EventArgs e)
        {
            //using CRL以获取扩展方法
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Where(b => b.Id > 0);
            query.Top(10);
            //选择GROUP字段

            //GROUP条件
            query.GroupBy(b => new { b.BarCode, b.ProductName });
            //having
            query.GroupHaving(b => b.Number.SUM() >= 0);
            //设置排序
            query.OrderBy(b => b.BarCode.Count(), true);//等效为 order by count(BarCode) desc
            var view = query.SelectV(b => new
            {
                b.BarCode,
                sum2 = b.SUM(x => x.Number * x.Id),//等效为 sum(Number*Id) as sum2
                total = b.BarCode.COUNT(),//等效为count(BarCode) as total
                sum1 = b.Number.SUM(),//等效为sum(Number) as sum1
                b.ProductName,
                avg = b.Number.AVG(),
            });
            txtOutput.Visible = true;
            txtOutput.Text = query.PrintQuery();
            var list = view.ToList();
            foreach (var item in list)
            {
                var str = string.Format("{0}______{1} {2} {3} {4}<br>", item.BarCode, item.ProductName, item.total, item.sum1, item.avg);//动态对象
                Response.Write(str);
            }
        }

        protected void Button8_Click(object sender, EventArgs e)
        {
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Where(b => b.Id > 0);
            var view = query.DistinctBy(b => new { b.ProductName, b.BarCode });
            //query.DistinctCount();//表示count Distinct 结果名为Total
            //var list = query.ToDynamic();
            var list = view.ToList();
            txtOutput.Visible = true;
            txtOutput.Text = query.PrintQuery();
            foreach (var item in list)
            {
                var str = string.Format("{0}______{1}<br>", item.ProductName, item.BarCode);//动态对象
                Response.Write(str);
            }
        }
    }
}
