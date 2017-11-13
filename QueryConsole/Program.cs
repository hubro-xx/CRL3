using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL;
namespace QueryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CRL.SettingConfig.GetDbAccess = (dbLocation) =>
            {
                var connString = CoreHelper.CustomSetting.GetConnectionString("default");
                return new CoreHelper.SqlHelper(connString);
            };
 
            var ja = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(Properties.Resources.String1);
            var return_code = ja["return_code"].ToString();
            var return_msg = ja["return_msg"].ToString();

            label1:

            Console.WriteLine("query CRL4");
            int n = 2;
            var n2 = GC.GetTotalMemory(true);
            var time = DateTime.Now;
            var sb = new StringBuilder();
            var data = new ProductData[]{new  ProductData()
                     };
            var query2 = ProductDataManage.Instance.GetLambdaQuery();
            query2.Top(1).Select(b=>new { Id = b.Id, sss2= b.Number});
            //var r1 = query2.ToSingle();
            //var v2=r1.Bag.sss2;
            //var str = "";
            for (int i = 0; i < 100; i++)
            {
                //sb.Append("ProductDataManageProductDataManage"+i);
                //str += "ProductDataManageProductDataManage";
                //var item = ProductDataManage.Instance.QueryItem(i);
                //var a = item == null;
                string sql = "sdfsf @sdfs=222 where id=" + i;
                var sql2 = System.Text.RegularExpressions.Regex.Replace(sql, @"@(\w+)", "?$1");
                continue;
                var query = ProductDataManage.Instance.GetLambdaQuery();
                query.Top(10);
                query.WithTrackingModel(false);
                query.Select(b => new { ss2 = (b.Id * b.Number.SUM() * 1).SUM() });
                //query.Select(b => new { ss = b.AddTime, b.Id, b.Style, b.SupplierId, b.CategoryName, b.ProductChannel });
                query.Where(b => b.Id > n && b.Id.Len()==10);
                //query.OrderBy(b => b.Id);
                ////query.Where(b => b.Id < 10);
                var str = query.ToString();
                //var result = query.ToList();
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
