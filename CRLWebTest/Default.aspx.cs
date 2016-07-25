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


        protected void Page_Load(object sender, EventArgs e)
        {

            //MongoDBTest.Test();
            TestAllQuery();

            //p.UserId += 1;//只会更新UserId
            //Code.ProductDataManage.Instance.Update(p);//按主键更新,主键值是必须的

            //var p2 = Code.ProductDataManage.Instance.QueryItem(b=>b.Id==p.Id);
            //Response.Write(p2.ToJson());
        }
       
        void TestJoin()
        {
            int id = 10;
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            var join = query.Join<Code.Order>((a, b) => a.Id == b.Id);
            join.SelectAppendValue(b => b.OrderId);
            join.Where(b => b.OrderId == "222");
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
