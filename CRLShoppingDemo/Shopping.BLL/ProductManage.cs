/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using Shopping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.BLL
{
    /// <summary>
    /// 产品管理
    /// </summary>
    public class ProductManage:CRL.BaseProvider<Product>
    {
        public static ProductManage Instance
        {
            get { return new ProductManage(); }
        }
        /// <summary>
        /// 重写缓存查询结果
        /// </summary>
        /// <returns></returns>
        protected override CRL.LambdaQuery.LambdaQuery<Product> CacheQuery()
        {
            var query = GetLambdaQuery();
            //关联出商家名称
            query.Join<Supplier>((a, b) => a.SupplierId == b.Id).SelectAppendValue(b => new { SupplierName = b.Name });
            var sql = query.PrintQuery();
            return query;
        }
    }
}
