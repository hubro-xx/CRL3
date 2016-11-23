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

namespace WebTest
{
    public partial class Cache : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        //首次查询后会创建缓存,下次再查询时,会按查询条件找到对应的缓存
        //以下缓存为异步更新,过期后会按条件异步重新查询最新数据,有线程单独维护
        //缓存在两个周期未使用后,会自动清理
        protected void Button1_Click(object sender, EventArgs e)
        {
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            //缓存会按条件不同缓存不同的数据,条件不固定时,慎用
            query.Where(b => b.Id < 700);
            int exp = 10;//过期分钟
            query.Expire(exp);
            var list = query.ToList();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            //默认过期时间为5分钟
            //AllCache可重写条件和过期时间,在业务类中实现即可
            //当插入或更新当前类型对象时,此缓存中对应的项也会更新
            var item = Code.ProductDataManage.Instance.QueryItemFromCache(5);
            var list = Code.ProductDataManage.Instance.QueryFromCache(b => b.Id < 10);
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            var list = Code.ProductDataManage.Instance.AllCache;//指定一个数据源
            #region 常规查找 多次计算和内存操作,增加成本
            var list2 = list.Where(b => b.Id > 0);//执行一次内存查找
            bool a = false;
            if (a)
            {
                list2 = list.Where(b => b.Number > 10);//执行第二次内存查找
            }
            #endregion

            #region 优化后查找 只需一次
            var query = new CRL.ExpressionJoin<Code.ProductData>(list, b => b.Id > 0);
            if (a)
            {
                query.And(b => b.Number > 10);//and 一个查询条件
            }
            list2 = query.ToList();//返回查询结果 只作一次内存查找
            #endregion
        }
    }
}
