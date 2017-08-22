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
            label1:

            Console.WriteLine("query CRL4");
            int n = 2;
            var n2 = GC.GetTotalMemory(true);
            var time = DateTime.Now;
            var sb = new StringBuilder();
            //var str = "";
            for (int i = 0; i < 100; i++)
            {
                //sb.Append("ProductDataManageProductDataManage"+i);
                //str += "ProductDataManageProductDataManage";
                var query = ProductDataManage.Instance.GetLambdaQuery();
                query.Top(i+1);
                query.WithTrackingModel(true);
                //query.Select(b => new { ss = b.AddTime, b.Id, b.Style, b.SupplierId, b.CategoryName, b.ProductChannel });
                query.Where(b => b.Id > n);
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
        class testA
        {
            public int key;
            public int value;
        }
    }
}
