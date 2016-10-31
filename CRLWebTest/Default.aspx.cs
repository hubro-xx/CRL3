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
namespace WebTest
{
    public partial class Default : System.Web.UI.Page
    {
        public class classA
        {
            public string name1
            {
                get;
                set;
            }
            public string name2
            {
                get;
                set;
            }
        }
        long Test()
        {
            var watch = new Stopwatch();
            var mainType = typeof(classA);
            watch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                //var obj=System.Runtime.Serialization.FormatterServices.GetUninitializedObject(mainType);
                var obj2 = System.Activator.CreateInstance(mainType);
            }
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }
        public static Code.ProductData data2
        {
            get
            {
                return new Code.ProductData() { Id = 10 };
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //var n=Test();
            //Response.Write(n);
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();

            query.Select(b => new
            {
                b.Id,b.ProductName
            });
            var year = DateTime.Now.Year;
            query.Where(b => b.Year == year);//虚拟字段

            query.Where(b => 0 < b.Id);//不再区分左边右边了
    
            query.OrderBy(b => b.Id * 1);
            var list = query.ToDictionary<int,string>();
            //TestJoin();
            //TestFileMapping();
            //Response.End();
            //MongoDBTest.Test();
            //TestAllQuery();
            //TestGuid();
            //p.UserId += 1;//只会更新UserId
            //Code.ProductDataManage.Instance.Update(p);//按主键更新,主键值是必须的

            //var p2 = Code.ProductDataManage.Instance.QueryItem(b=>b.Id==p.Id);
            //Response.Write(p2.ToJson());
        }

        void TestFileMapping()
        {
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Where(b => b.Id > 0);
            query.DistinctBy(b => new { b.ProductName });
            query.DistinctCount();//表示count Distinct 结果名为Total
            var list5 = query.ToDynamic();
            foreach (var item in list5)
            {
                var total = item.Total;
                //var name = item.ProductName;
            }
            var query2 = Code.ProductDataManage.Instance.GetLambdaQuery();
            query2.Select(b => new { name2 = b.ProductName, ss2 = b.PurchasePrice * b.Id });
            query2.Where(b => b.Id > 0);
            var result = query2.ToDynamic();
            foreach (var d in result)
            {
                var a = d.name2;
                var c = d.ss2;
            }
            var query3 = Code.ProductDataManage.Instance.GetLambdaQuery();
            query3.Join<Code.Member>((a, b) => a.UserId == b.Id).SelectAppendValue(b => b.Name);
            var resutl3 = query3.ToList();
            foreach (var d in resutl3)
            {
                var a = d.Bag.Name;
            }
        }
        void TestJoin()
        {
            int id = 10;
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            var join = query.Join<Code.Order>((a, b) => a.Id.ToString() == b.Id.ToString());
            join.SelectAppendValue(b => b.OrderId);
            join.Where(b => b.OrderId == id.ToString());
            var sql = query.PrintQuery();
            var result = query.ToList();
            Response.Write(sql);
            Response.End();
        }
        void TestGuid()
        {
            
            var c = new Code.ClassGuid();
            c.id = System.Guid.NewGuid();
            c.Name = "aaaa";
            Code.ClassGuidManage.Instance.Add(c);

            var list = Code.ClassGuidManage.Instance.QueryItem(c.id);
        }
        string getSecond
        {
            get
            {
                return Request["aa"];
            }
        }
        string aa()
        {
            return "123";
        }
        string filed1;
        void TestAllQuery()
        {
            string bc = "sdfsdf";
            filed1 = DateTime.Now.ToString();
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            //query.Select(b => new {aa=b.BarCode, b.CategoryName,cc=b.Id.SUM() });
            int c = 10;
            query.Where(b => !b.BarCode.Contains("123"));
            query.Where(b => !b.IsTop);
            query.Where(b => string.IsNullOrEmpty(b.BarCode));
            //query.Select(b => new { num = b.Number, aa = b.Id * b.Number, bb = 10, b.BarCode });//支持字段间二元运算
            //query.Select(b => b.Number.SUM());
            //query.Where(b => b.BarCode == aa());
            //query.Where(b => b.BarCode.Contains("abc"));
            //query.Join<Code.Member>((a, b) => a.UserId == b.Id).SelectAppendValue(b => b.Mobile);
            //query.Join<Code.Member>((a, b) => a.UserId == b.Id);
            //query.Select<Code.Member>((a, b) => new { a.UserId, b.Mobile });
            //query.GroupBy<Code.Member>((a, b) => new { a.BarCode, b.Name });
            //query.Where(b => b.Id < b.Id * b.Number && b.Id < b.Number);//支持二元运算
            //query.Where(b => c > b.Id);//不再区分左边右边了
            //query.Where(b => !b.BarCode.Like("%123"));//支持一元运算符判断了
            //query.Join<Code.Member>((a, b) => a.UserId == b.Id && a.Id == c, (a, b) => new { a.BarCode, Year2 = b.Year * b.Id });
            //query.AppendJoinValue<Code.Member>((a, b) => a.UserId == b.Id && a.Id == c, b => new { Year2 = b.Year * b.Id });
            //query.OrderBy<Code.Member>(b => b.Name, true);//支持关联对象字段排序了
            query.OrderBy(b => b.Id*1);
            var result = query.ToDynamic();
            var sql = query.PrintQuery();

        }
        
    }
}
