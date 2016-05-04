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
using CRL;
namespace WebTest.Code
{
    /// <summary>
    /// ProductData业务处理类
    /// 这里实现处理逻辑
    /// </summary>
    public class ProductDataManage : CRL.BaseProvider<ProductData>
    {
        /// <summary>
        /// 对象被更新时,是否通知缓存服务器
        /// </summary>
        protected override bool OnUpdateNotifyCacheServer
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 是否从远程查询缓存
        /// </summary>
        protected override bool QueryCacheFromRemote
        {
            get
            {
                return false;
            }
        }

       
        /// <summary>
        /// 实例访问入口
        /// </summary>
        public static ProductDataManage Instance
        {
            get { return new ProductDataManage(); }
        }

        public List<ProductData> QueryDayProduct(DateTime date)
        {
            var helper = DBExtend;
            string sql = "select * from ProductData where datediff(d,addtime,@date)=0";
            helper.AddParam("date", date);
            return helper.AutoSpQuery<ProductData>(sql);
            //其它数据结果参见Auto开头的其它方法
        }
        
        public void DynamicQueryTest()
        {
            //此方法演示根据结果集返回动态对象
            string sql = "select top 10 Id,ProductId,ProductName from ProductData";
            var helper = DBExtend;
            var list = helper.ExecDynamicList(sql);
            //添加引用 Miscorsoft.CSharp程序集
            foreach(dynamic item in list)
            {
                var id = item.Id;
                var productId = item.ProductId;
            }
        }
    }
}
