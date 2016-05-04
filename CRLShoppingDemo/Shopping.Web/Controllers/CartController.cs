/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using Shopping.BLL;
using Shopping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopping.Web.Controllers
{
    [Authorize(Roles="Member")]
    public class CartController : Core.Mvc.BaseController
    {
        /// <summary>
        /// 购物车首页
        /// </summary>
        /// <param name="cartType"></param>
        /// <returns></returns>
        public ActionResult Index()
        {
            var list = GetCart();
            return View(list);
        }
        List<CartItem> GetCart()
        {
            int userId = CurrentUser.Id;
            var items = CartManage.Instance.QueryCart(userId, false);
            decimal total = 0;
            foreach (var item in items)
            {
                if (!item.Selected)
                    continue;
                total += item.Num * item.Price;
            }
            ViewBag.total = total;
            return items;
        }

        public ActionResult GetCartProductCount()
        {
            int userId = CurrentUser.Id;
            var count = CartManage.Instance.GetCartCount(userId);
            return JsonResult(true, "", count);
        }
        public string GetCountCartTotal()
        {
            int userId = CurrentUser.Id;
            decimal result = CartManage.Instance.GetTotalAmount(userId);
            return result.ToString("F");
        }

        #region 加入购物车

        /// <summary>
        /// 添加到购物车,默认购物车
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddToCart(int productId,  int num)
        {
            string message;
            if(CurrentUser==null)
            {
                return JsonResult(false,"需要登录");
            }
            bool a = AddToCart(productId, num, out message);
            return JsonResult(a, message);
        }
        public bool AddToCart(int productId, int num, out string error)
        {
            int userId = CurrentUser.Id;
            error = "";
            bool result = false;
            var product = BLL.ProductManage.Instance.QueryItem(productId);
            if (product == null)
            {
                error = "找不到产品";
                return false;
            }
            if (product.ProductStatus != CRL.Package.Product.ProductStatus.已上架)
            {
                error = "非常抱歉！该商品现已下架！";
                return false;
            }
            
            bool a = CartManage.Instance.AddToCart(userId, product, num, out error);
            result = a;
            return a;
        }

        #endregion
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            int userId = CurrentUser.Id;
            bool a = CartManage.Instance.Remove(userId, id);
            return JsonResult(true, "", GetCountCartTotal());
        }
        /// <summary>
        /// 更改数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeCartNum(int id, int num)
        {
            var userId = CurrentUser.Id;
            CartManage.Instance.ChangeNum(userId, num, id);
            return JsonResult(true, "", GetCountCartTotal());
        }
        /// <summary>
        /// 设置选中
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SelectItem(int id, bool selected)
        {
            int userId = CurrentUser.Id;
            CartManage.Instance.SetSelected(userId, id, selected);
            return JsonResult(true, "", GetCountCartTotal());
        }
        [HttpPost]
        public ActionResult ClearCart()
        {
            int userId = CurrentUser.Id;
            CartManage.Instance.RemoveAll(userId);
            return JsonResult(true, "", 0);
        }
    }
}
