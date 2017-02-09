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
using WebTest.Code;
namespace WebTest
{
    public partial class Update : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //更新传值有多种方式

            //字典传参的形式
            CRL.ParameCollection c = new CRL.ParameCollection();
            c["ProductName"] = "product1";
            Code.ProductDataManage.Instance.Update(b => b.Id == 4, c);
            //按匿名对象
            Code.ProductDataManage.Instance.Update(b => b.Id == 4, new { ProductName = "product1" });

            //按对象差异更新
            var p = new Code.ProductData() { Id = 4 };
            //手动修改值时,指定修改属性以在Update时识别,分以下几种形式
            p.Change(b => b.BarCode);//表示值被更改了
            p.Change(b => b.BarCode, "123");//通过参数赋值
            p.Change(b => b.BarCode == "123");//通过表达式赋值
            p.Cumulation(b => b.ProductName, "1");//表示按字段累加
            Code.ProductDataManage.Instance.Update(b => b.Id == 4, p);//指定查询更新

            //当对象是查询创建则能自动识别
            p = Code.ProductDataManage.Instance.QueryItem(b => b.Id > 0);
            p.UserId += 1;//只会更新UserId
            p.ProductName = "2342342";
            Code.ProductDataManage.Instance.Update(p);//按主键更新,主键值是必须的

            //使用完整查询关联更新
            var query = Code.OrderManage.Instance.GetLambdaQuery();
            query.Join<Code.ProductData>((a, b) => a.Id == b.Id && b.Number > 10);
            c = new CRL.ParameCollection();
            c["UserId"] = "$UserId";//order.userid=product.userid
            c["Remark"] = "2222";//order.remark=2222
            Code.OrderManage.Instance.Update(query, c);
            //等效语句为 update order set userid=ProductData.userid,remark='2222' from ProductData where order.id=ProductData.id and ProductData.number<10
        }
        public static void TestModified()
        {
            var item2 = ProductDataManage.Instance.QueryItemFromCache(b => b.Id > 0);
            item2.BarCode = "23424234";
            var c1 = item2.IsModified();
            ProductDataManage.Instance.Update(item2);
            var c2 = item2.IsModified();
            item2.Change(b => b.BarCode, "2222");
            var c3 = item2.IsModified();
            ProductDataManage.Instance.Update(item2);
            var c4 = item2.IsModified();
        }
    }
}
