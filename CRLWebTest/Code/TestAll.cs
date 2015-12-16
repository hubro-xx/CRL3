using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL;
namespace WebTest.Code
{
    public class TestAll
    {
        public static void TestQuery()
        {
            var instance = Code.ProductDataManage.Instance;
            var query = ProductDataManage.Instance.GetLambdaQuery();
            query.Select(b => new { b.InterFaceUser, bb = b.Id * b.Number
                });
            var year = DateTime.Now.Year;
            query.Where(b => b.Year == year);//虚拟字段
            #region 扩展方法
            query.Where(b => 0 < b.Id);//不再区分左边右边了
            query.Where(b => b.Id < b.Number);//直接比较可以解析通过
            query.Where(b => b.ProductName.Contains("122"));//包含字符串
            query.Where(b => !b.ProductName.Contains("122"));//不包含字符串
            query.Where(b => b.ProductName.In("111", "222"));//string in
            query.Where(b => b.AddTime.Between(DateTime.Now, DateTime.Now));//在时间段内
            query.Where(b => b.AddTime.DateDiff(DatePart.dd, DateTime.Now) > 1);//时间比较
            query.Where(b => b.ProductName.Substring(0, 3) == "222");//截取字符串
            query.Where(b => b.Id.In(1, 2, 3));//in
            query.Where(b => !b.Id.In(1, 2, 3));//not in
            query.Where(b => b.UserId.Equals(Code.ProductChannel.其它));//按值等于,enum等于int
            query.Where(b => b.ProductName.StartsWith("abc"));//开头值判断
            query.Where(b => b.Id.Between(1, 10));//数字区间
            query.Where(b => b.ProductName.Like("123"));// %like%
            query.Where(b => b.ProductName.LikeLeft("123"));// %like
            query.Where(b => b.ProductName.LikeRight("123"));// like%
            query.Page(2, 1);
            var sql1 = query.PrintQuery();
            var list = query.ToDynamic();
            #endregion

            #region 关联
            //索引值
            query = ProductDataManage.Instance.GetLambdaQuery();
            query.Join<Code.Member>((a, b) => a.UserId == b.Id && a.BarCode.Contains("1"))
                .SelectAppendValue(b => b.Mobile).OrderBy(b => b.Id, true);
            var list2 = query.ToList();
            foreach (var item in list2)
            {
                var mobile = item["Mobile"];
            }
            //按筛选值
            query = ProductDataManage.Instance.GetLambdaQuery();
            query.Join<Code.Member>((a, b) => a.UserId == b.Id && a.BarCode.Contains("1"))
                .Select((a, b) => new { a.BarCode, b.Name }).Join<Code.Order>((a, b) => a.Id == b.UserId);
            query.OrderBy(b => b.Id);
            var list3 = query.ToDynamic();
            foreach (var item in list3)
            {
                var barCode = item.BarCode;
            }
            //关联再关联
            query = ProductDataManage.Instance.GetLambdaQuery();
            query.Join<Code.Member>((a, b) => a.UserId == b.Id && a.BarCode.Contains("1"))
                .SelectAppendValue(b => b.Mobile).OrderBy(b => b.Id, true).Join<Code.Order>((a, b) => a.Id == b.UserId);
            //按IN查询
            query = ProductDataManage.Instance.GetLambdaQuery();
            query.In<Code.Member>(b => b.UserId, b => b.Id, (a, b) => a.SupplierId == "10" && b.Name == "123");
            var sql2 = query.PrintQuery();
            #endregion

            #region GROUP
            query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Where(b => b.Id > 0);
            //选择GROUP字段
            query.Select(b => new
            {
                sum2 = b.SUM(x => x.Number * x.Id),//等效为 sum(Number*Id) as sum2
                total = b.BarCode.COUNT(),//等效为count(BarCode) as total
                sum11 = b.Number.SUM(),//等效为sum(Number) as sum1
                b.ProductName,
                num1 = b.SUM(x => x.Number * x.Id),
                num2 = b.MAX(x => x.Number * x.Id),
                num3 = b.MIN(x => x.Number * x.Id),
                num4 = b.AVG(x => x.Number * x.Id) 
            });
            //GROUP条件
            query.GroupBy(b => new { b.ProductName });
            //having
            query.GroupHaving(b => b.Number.SUM() >= 0);
            //设置排序
            query.OrderBy(b => b.BarCode.Count(), true);//等效为 order by count(BarCode) desc
            var list4 = query.ToDynamic();
            foreach(var item in list4)
            {
                var total = item.total;
            }
            #endregion

            #region DISTINCT
            query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Where(b => b.Id > 0);
            query.DistinctBy(b => new { b.ProductName });
            query.DistinctCount();//表示count Distinct 结果名为Total
            var list5 = query.ToDynamic();
            foreach (var item in list5)
            {
                var total = item.Total;
                //var name = item.ProductName;
            }
            #endregion

            #region 函数
            //按条件id>0,合计Number列
            var sum = instance.Sum(b => b.Id > 0, b => b.Number * b.UserId);
            //按条件id>0,进行总计
            var count = instance.Count(b => b.Id > 0);
            var max = instance.Max(b => b.Id > 0, b => b.Id);
            var min = instance.Min(b => b.Id > 0, b => b.Id);
            //使用语句进行函数查询
            query = ProductDataManage.Instance.GetLambdaQuery();
            query.Select(b => b.Number.SUM());
            decimal sum2 = query.ToScalar();
            #endregion
        }
        public static void TestUpdate()
        {
            var instance = Code.ProductDataManage.Instance;
            #region 更新
            //要更新属性集合
            CRL.ParameCollection c = new CRL.ParameCollection();
            c["ProductName"] = "product1";
            Code.ProductDataManage.Instance.Update(b => b.Id == 4, c);
            //按对象差异更新
            var p = new Code.ProductData() { Id = 4 };
            //手动修改值时,指定修改属性以在Update时识别,分以下几种形式
            p.Change(b => b.BarCode);//表示值被更改了
            p.Change(b => b.BarCode, "123");//通过参数赋值
            p.Change(b => b.BarCode == "123");//通过表达式赋值
            Code.ProductDataManage.Instance.Update(b => b.Id == 4, p);//指定查询更新

            p = Code.ProductDataManage.Instance.QueryItem(b => b.Id > 0);
            p.UserId += 1;
            Code.ProductDataManage.Instance.Update(p);//按主键更新,主键值是必须的
            #endregion

            #region 缓存更新
            var item = Code.ProductDataManage.Instance.QueryItemFromCache(1);
            var guid = Guid.NewGuid().ToString().Substring(0,8);
            item.Change(b => b.SupplierName, guid);
            Code.ProductDataManage.Instance.Update(item);
            item = Code.ProductDataManage.Instance.QueryItemFromCache(1);
            var item2 = Code.ProductDataManage.Instance.QueryItem(1);
            var a = item.SupplierName == item2.SupplierName && item.SupplierName == guid;
            if (!a)
            {
                throw new Exception("更新缓存失败");
            }
            #endregion

            #region 事务
            string error;
            item = Code.ProductDataManage.Instance.QueryItem(1);

            var result = Code.ProductDataManage.Instance.PackageTrans((out string ex) =>
            {
                ex = "";
                var product = new ProductData();
                product.BarCode = "sdfsdf";
                product.Number = 10;
                ProductDataManage.Instance.Add(product);
                return false;
            }, out error);
            if (result)
            {
                throw new Exception("事务未回滚");
            }
            #endregion
        }
    }
}