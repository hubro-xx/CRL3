/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RoleControl.Models
{
    public class Pager
    {
        /// <summary>
        /// 创建分页导航
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public static MvcHtmlString Create<T>(PageObj<T> page)
        {
            return Create(page, CoreHelper.PageNavigation.PageStyle.Default, "");
        }
        /// <summary>
        /// 创建分页导航
        /// </summary>
        public static MvcHtmlString Create<T>(PageObj<T> page, CoreHelper.PageNavigation.PageStyle style, string navFormat)
        {
            var nav = new CoreHelper.PageNavigation();
            nav.SetPageStyle(style);
            nav.PageNavigationFormat = navFormat;
            if (page.Count == 0)
            {
                return MvcHtmlString.Create("");
            }
            string str = nav.GetPageNavigation(page.PageIndex, page.Total, page.PageSize);
            return MvcHtmlString.Create(str);
        }
    }
    public class PageObj<T> : List<T>, IEnumerable<T>, IEnumerable
    {
        public PageObj(IEnumerable<T> allItems, int pageIndex, int total, int pageSize)
        {
            AddRange(allItems);
            PageIndex = pageIndex;
            Total = total;
            PageSize = pageSize;
        }
        public int PageIndex
        {
            get;
            set;
        }
        public int Total
        {
            get;
            set;
        }
        public int PageSize
        {
            get;
            set;
        }
    }
}
