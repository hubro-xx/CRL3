/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTest.Code
{
    /// <summary>
    /// OrderManage
    /// </summary>
    public class OrderManage : CRL.BaseProvider<Order>
    {
        public static OrderManage Instance
        {
            get { return new OrderManage(); }
        }
        public void TestRelationUpdate()
        {
            var query = GetLambdaQuery();
            query.Join<ProductData>((a, b) => a.Id == b.Id && b.Number > 10);
            var c = new CRL.ParameCollection();
            c["UserId"] = "$UserId";//order.userid=product.userid
            c["Remark"] = "2222";//order.remark=2222
            Update(query, c);
        }
        public bool TransactionTest2(out string error)
        {
            var db = DBExtend;
            //简化了事务写法,自动提交回滚
            return PackageTrans2(db,(out string ex) =>
            {
                ex = "";
                var product = new ProductData();
                product.BarCode = "code" + DateTime.Now.Millisecond;
                product.Number = 10;
                db.InsertFromObj(product);
                return false; //会回滚
            }, out error);
        }
        public bool TransactionTest(out string error)
        {
            //简化了事务写法,自动提交回滚
            return PackageTrans((out string ex) =>
            {
                ex = "";
                var product = new ProductData();
                product.BarCode = "code" + DateTime.Now.Millisecond;
                product.Number = 10;
                ProductDataManage.Instance.Add(product);
                return false; //会回滚
            }, out error);
        }
    }
}
