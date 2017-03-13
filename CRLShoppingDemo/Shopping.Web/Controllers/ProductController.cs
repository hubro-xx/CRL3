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
using System.Web.Mvc;
using Shopping.BLL;
using Shopping.Model;
namespace Shopping.Web.Controllers
{
    public class ProductController : Core.Mvc.BaseController
    {
        public ActionResult Index(string k, string c = "", int page = 1, int pageSize = 15)
        {
            int count;

            count = 0;
            //使用缓存搜索
            var query = new CRL.ExpressionJoin<Product>(BLL.ProductManage.Instance.AllCache, b => b.ProductStatus == CRL.Package.Product.ProductStatus.已上架);
            if (!string.IsNullOrEmpty(k))
            {
                query.And(b => b.ProductName.Contains(k));
            }
            if (!string.IsNullOrEmpty(c))
            {
                query.And(b => b.CategoryCode.StartsWith(c));
            }

            var products = query.ToList().OrderByDescending(b => b.Id);
            count = products.Count();
            var result = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var pageObj = new PageObj<Product>(result, page, count, pageSize);
            return View(pageObj);
        }

        public ActionResult Item(int id)
        {
            var item = ProductManage.Instance.QueryItemFromCache(b => b.Id == id);
            if (item == null)
            {
                return Content("not found");
            }
            return View(item);
        }
    }
}
