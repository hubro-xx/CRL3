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
namespace WebTest.Page
{
    public partial class Extension : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //using CRL 以获取扩展方法
            //对于一元运算,可按!=判断,如b.ProductName.Contains("122") 和!b.ProductName.Contains("122")
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
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
            query.Where(b => !string.IsNullOrEmpty(b.BarCode));
            int LastDays = 30;
            query.Where(b => b.AddTime.DateDiff(CRL.DatePart.dd, DateTime.Now) < LastDays);
            var result = query.ToList();
            var sql = query.ToString();
            Response.Write(sql);
            //var list = Code.ProductDataManage.Instance.QueryList(query);
        }
    }
}
