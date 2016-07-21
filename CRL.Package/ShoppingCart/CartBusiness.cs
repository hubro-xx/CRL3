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
using System.Data;
using System.Reflection;

namespace CRL.Package.ShoppingCart
{
    /// <summary>
    /// 购物车维护
    /// </summary>
    public class CartBusiness<TType, TModel> : BaseProvider<TModel> where TType : class where TModel:CartItem,new()
    {
        /// <summary>
        /// 购物车数量(选中)
        /// </summary>
        public int GetCartCount(int userId, int cartType = 0)
        {
            if (userId > 0)
            {
                int a = Sum<int>(b => b.UserId == userId && b.CartType == cartType&&b.Selected==true, b => b.Num, true);
                return a;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取购物车总额
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cartType"></param>
        /// <returns></returns>
        public decimal GetTotalAmount(int userId, int cartType=0)
        {
            var helper = DBExtend;

            var a = Sum<decimal>(b => b.UserId == userId && b.CartType == cartType && b.Selected == true, b => b.Num * b.Price);
            return a;
        }

        /// <summary>
        /// 添加到购物车
        /// </summary>
        /// <param name="item"></param>
        /// <param name="groupByPrice">是否按价格分组</param>
        /// <returns></returns>
        public int Add(TModel item,bool groupByPrice=false)
        {
            var helper = DBExtend;
            TModel item2;
            if (groupByPrice)//按价格分组
            {
                item2 = helper.QueryItem<TModel>(b => b.UserId == item.UserId && b.ProductId == item.ProductId && b.StyleId == item.StyleId && b.CartType == item.CartType && b.Price == item.Price);
            }
            else
            {
                item2 = helper.QueryItem<TModel>(b => b.UserId == item.UserId && b.ProductId == item.ProductId && b.StyleId == item.StyleId && b.CartType == item.CartType);
            }
            if (item2==null)
            {
                //int n = GetCartCount(item.CartType) + 1;
                //SetCartCount(item.CartType,n);
                base.Add(item);
            }
            else
            {
                AddNum(item);
            }
            return GetCartCount(item.CartType);
        }
        /// <summary>
        /// 更改数量
        /// </summary>
        /// <param name="item"></param>
        public void AddNum(TModel item)
        {
            ParameCollection c = new ParameCollection();
            c["$Num"] = "Num+" + item.Num;
            //int n = GetCartCount(item.CartType) + item.Num;
            //SetCartCount(item.CartType, n);
            Update(b => b.UserId == item.UserId && b.ProductId == item.ProductId && b.StyleId == item.StyleId && b.CartType == item.CartType, c);
        }
        /// <summary>
        /// 更改数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="num"></param>
        /// <param name="id"></param>
        public void ChangeNum(int userId,int num, int id)
        {
            var cartItem = QueryItem(b => b.UserId == userId && b.Id == id);
            if (cartItem != null)
            {
                //int n = GetCartCount(cartItem.CartType) + num - cartItem.Num;
                //SetCartCount(cartItem.CartType, n);
            }
            ParameCollection c = new ParameCollection();
            c["Num"] = num;
            Update(b => b.UserId == userId && b.Id == id, c);
        }
        public void UpdatePrice(int userId, int id, decimal price)
        {
            ParameCollection c = new ParameCollection();
            c["price"] = price;
            Update(b => b.UserId == userId && b.Id == id, c);
        }
        public CartItem GetCartItem(int userId, int id)
        {
            return QueryItem(b => b.UserId == userId && b.Id == id);
        }
        /// <summary>
        /// 设置是否选中
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <param name="selected"></param>
        public void SetSelected(int userId, int id, bool selected)
        {
            ParameCollection c = new ParameCollection();
            c["Selected"] = selected;
            Update(b => b.UserId == userId && b.Id == id, c);
            //int n = GetCartCount(cartType);
            //SetCartCount(cartType, n);
        }
        /// <summary>
        /// 移除多项,不移除没选中的
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cartType"></param>
        public void RemoveAll(int userId, int cartType=0)
        {
            Delete(b => b.UserId == userId && b.CartType == cartType && b.Selected == true);
            //int n = GetCartCount(cartType);
            //SetCartCount(cartType, n);
        }
        /// <summary>
        /// 移除一项
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        public bool Remove(int userId, int id)
        {
            return Delete(b => b.UserId == userId && b.Id == id) > 0;
        }
    }
}
