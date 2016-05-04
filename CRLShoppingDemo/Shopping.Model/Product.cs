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
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Model
{
    /// <summary>
    /// 产品
    /// </summary>
    public class Product : CRL.Package.Product.ProductBase
    {
        /// <summary>
        /// 初始默认数据
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.IList GetInitData()
        {
            var list = new List<Product>();
            for (int i = 1; i < 100; i++)
            {
                list.Add(new Product() { ProductName = "测试产品" + i, SettlementPrice = 80, SoldPrice = 100, SupplierId = 1, ProductStatus = CRL.Package.Product.ProductStatus.已上架 });
            }
            return list;
        }
        /// <summary>
        /// 数据校验
        /// </summary>
        /// <returns></returns>
        public override string CheckData()
        {
            if (SupplierId <= 0)
            {
                return "商家编号为0";
            }
            if (SoldPrice <= 0)
            {
                return "售价不能为0";
            }
            return base.CheckData();
        }
    }
}
