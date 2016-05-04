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
    /// 购物车管理
    /// </summary>
    public class CartManage : CRL.Package.ShoppingCart.CartBusiness<CartManage, CartItem>
    {
        public static CartManage Instance
        {
            get { return new CartManage(); }
        }
        /// <summary>
        /// 查询购物车
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="onlySelected"></param>
        /// <param name="_cartType"></param>
        /// <returns></returns>
        public List<CartItem> QueryCart(int userId, bool onlySelected, int _cartType=0)
        {
            var query = GetLambdaQuery();
            query.Where(b => b.UserId == userId && b.CartType == _cartType);
            if (onlySelected)
            {
                query.Where(b => b.Selected == true);
            }
            var list = query.ToList();
            return list;
        }
        /// <summary>
        /// 添加到购物车
        /// </summary>
        public bool AddToCart(int userId, Product product,  int num, out string error)
        {
            error = "";
            var c = new CartItem();
            c.Price = product.SoldPrice;
            c.Num = num;
            c.ProductId = product.Id;
            c.ProductName = product.ProductName;
            c.StyleId = 0;
            c.StyleName = "";
            c.SupplierId = product.SupplierId;
            c.UserId = userId;
            Add(c, false);
            return true;
        }
    }
}
