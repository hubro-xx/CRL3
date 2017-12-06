/**
* CRL 快速开发框架 V4.5
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL;
using System.Text.RegularExpressions;

namespace QueryConsole
{
    class Program
    {
        static int queueInitStatus = 0;
        static object lockObj = new object();

        static void Main(string[] args)
        {
            CRL.SettingConfig.GetDbAccess = (dbLocation) =>
            {
                var connString = CoreHelper.CustomSetting.GetConnectionString("default");
                return new CoreHelper.SqlHelper(connString);
            };
            //ProductDataManage.Instance.Test();
            var resutl2 = ProductDataManage.Instance.QueryFromCache(b => b.Id<10);
            return;
            label1:

            Console.WriteLine("query CRL4");
            int n = 2;
            var n2 = GC.GetTotalMemory(true);
            var time = DateTime.Now;
            
            //var r1 = query2.ToSingle();
            //var v2=r1.Bag.sss2;
            //var str = "";
            for (int i = 0; i < 100; i++)
            {
                //sb.Append("ProductDataManageProductDataManage"+i);
                //str += "ProductDataManageProductDataManage";
                //var item = ProductDataManage.Instance.QueryItem(i);
                //var a = item == null;
                var query = ProductDataManage.Instance.GetLambdaQuery();
                query.Top(10);
                query.WithTrackingModel(false);
                query.Select(b => new { ss2 = (b.Id * b.Number.SUM() * 1).SUM() });
                //query.Select(b => new { ss = b.AddTime, b.Id, b.Style, b.SupplierId, b.CategoryName, b.ProductChannel });
                query.Where(b => b.Id > n && b.Id.Len()==10);
                //query.OrderBy(b => b.Id);
                ////query.Where(b => b.Id < 10);
                var str = query.ToString();
                var result = query.ToList();
                //query.Where(b => b.ProductId == "333" && b.PurchasePrice < 100);
                //n += list.Count;
                //list = null;
                //var list = ProductDataManage.Instance.QueryList(b => b.Id == i);
                //list = null;
            }

            var ts = DateTime.Now - time;
            var n3 = GC.GetTotalMemory(false) - n2;
            Console.WriteLine("ok " + n3 / 1024 + " " + ts.TotalMilliseconds);
            Console.ReadLine();
            goto label1;
        }
        //[CRL.Attribute.ModelProxy]
        class testA:CRL.IModel
        {
            public int key
            {
                get;set;
            }
            public int value
            {
                get;set;
            }
            int aa;
        }
        class testB:testA
        {
            public int a
            {
                get
                {
                    return 1;
                }
            }
        }
    }
}
