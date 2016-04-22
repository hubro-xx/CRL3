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
            var db = DBExtend;
            var c = new CRL.ParameCollection();
            c["UserId"] = "$UserId";//order.userid=product.userid
            c["Remark"] = "2222";//order.remark=2222
            RelationUpdate<ProductData>((a, b) => a.Id == b.Id && b.Number > 10, c);
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