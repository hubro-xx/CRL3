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
using Shopping.Model;
using Shopping.BLL;
using CRL;
namespace Shopping.Web.Controllers
{
    [Authorize(Roles = "Supplier")]
    public class SupplierController : Core.Mvc.BaseController
    {
        //
        // GET: /Supplier/

        public ActionResult Index()
        {
            return View();
        }
        #region 登录
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Supplier supplier)
        {
            string error;
            var a = SupplierManage.Instance.CheckPass(supplier.AccountNo, supplier.PassWord, out error);
            if (!a)
            {
                ModelState.AddModelError("", error);
                return View();
            }
            var u = SupplierManage.Instance.QueryItem(b => b.AccountNo == supplier.AccountNo);
            if (u.Locked)
            {
                ModelState.AddModelError("", "账号已锁定");
                return View();
            }
            SupplierManage.Instance.Login(u, "Supplier", false);
            string returnUrl = Request["returnUrl"];
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/";
            }
            return Redirect(returnUrl);
        }
        #endregion
        public ActionResult Product(int page = 1, int pageSize = 15)
        {
            var query = ProductManage.Instance.GetLambdaQuery();
            query.Where(b => b.SupplierId == CurrentUser.Id);
            query.Page(pageSize, page);
            query.OrderBy(b => b.Id, false);
            var result = query.ToList();
            int count = query.RowCount;
            var pageObj = new PageObj<Product>(result, page, count, pageSize);
            return View(pageObj);
        }

        public ActionResult ProductUpdate(int id)
        {
            var item = ProductManage.Instance.QueryItem(id);
            return View(item);
        }
        [HttpPost]
        public ActionResult ProductUpdate(Product product)
        {
            //因为product是MVC创建的,需要手动表示哪些值被更改了
            product.Change(b => b.ProductName);
            product.Change(b => b.SettlementPrice);
            product.Change(b => b.SoldPrice);
            product.Change(b => b.ProductStatus);
            ProductManage.Instance.Update(product);
            return RedirectToAction("Product");
        }
        public ActionResult ProductDelete(int id)
        {
            ProductManage.Instance.Delete(id);
            return RedirectToAction("Product");
        }
    }
}
