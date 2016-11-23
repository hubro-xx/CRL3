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
using CRL;
namespace WebTest.Code
{
    public class TestAll
    {
        public static void TestQuery()
        {
            var instance = Code.ProductDataManage.Instance;
            instance.QueryItem(1);
            var query = ProductDataManage.Instance.GetLambdaQuery();
            query.Where(b => b.TransType == TransType.In);
           
            var year = DateTime.Now.Year;
            query.Where(b => b.Year == year);//虚拟字段
            #region 扩展方法
            query.Where(b => b.IsTop);//没有运算符的bool一元运算
            query.Where(b => 0 < b.Id);//不再区分左边右边了
            query.Where(b => b.Id < b.Number);//直接比较可以解析通过
            query.Where(b => b.ProductName.Contains("122"));//包含字符串
            query.Where(b => !b.ProductName.Contains("122"));//不包含字符串
            query.Where(b => b.CategoryName.Contains(b.BarCode));//支持属性调用了
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
            query.Where(b => b.Id.ToString() == "123");//支持Cast转换
            query.Page(2, 1);
            query.OrderBy(b => b.Id * 1);
            var result = query.SelectV(b => new
            {
                b.InterFaceUser,
                bb = b.Id * b.Number,
                b.ProductName
            }).ToList();
            var sql1 = query.PrintQuery();
            #endregion
        }
        public static void TestView()
        {
            var q1 = Code.OrderManage.Instance.GetLambdaQuery();
            var q2 = q1.CreateQuery<Code.ProductData>();
            q2.Where(b => b.Id > 0);
            var view = q2.CreateQuery<Code.Member>().GroupBy(b => b.Name).Where(b => b.Id > 0).SelectV(b => new { b.Name, aa = b.Id.COUNT() });//GROUP查询
            var view2 = q2.Join(view, (a, b) => a.CategoryName == b.Name).Select((a, b) => new { ss1 = a.UserId, ss2 = b.aa });//关联GROUP
            q1.Join(view2, (a, b) => a.Id == b.ss1).Select((a, b) => new { a.Id, b.ss1 });//再关联
            //var result = view2.ToList();
            var sql = q1.ToString();
        }
        public static void TestJoin()
        {
            #region 关联
            //索引值
            var query = ProductDataManage.Instance.GetLambdaQuery();
            var join = query.Join<Code.Member>((a, b) => a.UserId == b.Id && a.BarCode.Contains("1"))
                .SelectAppendValue(b => b.Mobile).OrderBy(b => b.Id, true);
            join.Where(b => b.AccountNo == "123");//按join追加条件
            var list2 = query.ToList();
            foreach (var item in list2)
            {
                var mobile = item["Mobile"];
                var mobile2 = item.Bag.Mobile;
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
            var query2 = query.CreateQuery<Code.Member>();
            var view = query2.Where(b => b.Name == "123").SelectV(b => b.Id);
            query.In(view, b => b.UserId);
            var sql2 = query.PrintQuery();
            //按exists
            query = ProductDataManage.Instance.GetLambdaQuery();
            query2 = query.CreateQuery<Code.Member>();
            var view2 = query2.Where(b => b.Name == "123").SelectV(b => b.Id);
            query.Exists(view2);
            sql2 = query.PrintQuery();
            #endregion
        }
        public static void TestGroup()
        {
            #region GROUP
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
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
            foreach (var item in list4)
            {
                var total = item.total;
            }
            #endregion
        }
        public static void TestDistinct()
        {
            #region DISTINCT
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            query.Where(b => b.Id > 0);
            query.DistinctBy(b => new { b.ProductName });
            //query.DistinctCount();//表示count Distinct 结果名为Total
            var list5 = query.ToDynamic();
            foreach (var item in list5)
            {
                //var total = item.Total;
                var name = item.ProductName;
            }
            #endregion
        }
        public static void TestUnion()
        {
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            var query2 = query.CreateQuery<Code.Order>();
            var view1 = query.SelectV(b => new { a1 = b.Id, a2 = b.ProductName });
            var view2 = query2.SelectV(b => new { a1 = b.Id, a2 = b.Remark });
            var result = view1.Union(view2).OrderBy(b => b.a1).OrderBy(b => b.a2, false).ToList();
            string sql = query.ToString();
        }
        public static void TestFunc()
        {
            #region 函数
            var instance = Code.ProductDataManage.Instance;
            //按条件id>0,合计Number列
            var sum = instance.Sum(b => b.Id > 0, b => b.Number * b.UserId);
            //按条件id>0,进行总计
            var count = instance.Count(b => b.Id > 0);
            var max = instance.Max(b => b.Id > 0, b => b.Id);
            var min = instance.Min(b => b.Id > 0, b => b.Id);
            //使用语句进行函数查询
            var query = ProductDataManage.Instance.GetLambdaQuery();
            query.Select(b => b.Number.SUM());
            decimal sum2 = query.ToScalar();
            #endregion
        }

        public static void TestFileMapping()
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

            //关联更新
            var query = Code.OrderManage.Instance.GetLambdaQuery();
            query.Join<ProductData>((a, b) => a.Id == b.Id && b.Number > 10);
            c = new CRL.ParameCollection();
            c["UserId"] = "$UserId";//order.userid=product.userid
            c["Remark"] = "2222";//order.remark=2222
            Code.OrderManage.Instance.Update(query, c);
            
            #endregion

            #region 删除
            //关联删除
            var query2 = Code.ProductDataManage.Instance.GetLambdaQuery();
            query2.Where(b => b.Id == 10);
            query2.Join<Code.Member>((a, b) => a.SupplierId == "10" && b.Name == "123");
            Code.ProductDataManage.Instance.Delete(query2);
            Code.ProductDataManage.Instance.Delete(999);
            #endregion

            #region 缓存更新
            //按编号为1的数据
            var item = Code.ProductDataManage.Instance.QueryItemFromCache(b => b.Id > 0);
            item.CheckNull("item");
            var guid = Guid.NewGuid().ToString().Substring(0,8);
            item.Change(b => b.SupplierName, guid);
            Code.ProductDataManage.Instance.Update(item);
            item = Code.ProductDataManage.Instance.QueryItemFromCache(item.Id);
            var item2 = Code.ProductDataManage.Instance.QueryItem(item.Id);
            var a2 = item.SupplierName == item2.SupplierName && item.SupplierName == guid;
            if (!a2)
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
