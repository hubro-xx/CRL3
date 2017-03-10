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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Core.Mvc
{
    public static partial class HtmlExtensions
    {
        public static MvcHtmlString Pager<T>(this HtmlHelper html, PageObj<T> pageObj, CoreHelper.PageNavigation.PageStyle style = CoreHelper.PageNavigation.PageStyle.Google, string navFormat = "")
        {
            return Core.Mvc.ControllHelper.CreatePageNavigation(pageObj, style, navFormat);
        }

        /// <summary>
        /// 按枚举创建已选择的DropDownList
        /// </summary>
        /// <param name="html"></param>
        /// <param name="name"></param>
        /// <param name="selectValue"></param>
        /// <param name="nullOption"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListEx(this HtmlHelper html, string name, Enum selectValue, bool nullOption = true, object htmlAttributes = null)
        {
            return Core.Mvc.ControllHelper.DropDownList(name, selectValue, nullOption, htmlAttributes);
        }
        /// <summary>
        /// 按request值创建已选择的DropDownList
        /// </summary>
        /// <param name="html"></param>
        /// <param name="type"></param>
        /// <param name="requestName"></param>
        /// <param name="nullOption"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListEx(this HtmlHelper html, Enum type, string requestName, bool nullOption = true, object htmlAttributes = null)
        {
            return Core.Mvc.ControllHelper.DropDownList(type, requestName, nullOption, htmlAttributes);
        }
        /// <summary>
        /// 自定义创建DropDownList
        /// </summary>
        /// <param name="html"></param>
        /// <param name="name"></param>
        /// <param name="items"></param>
        /// <param name="nullOption"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListEx(this HtmlHelper html, string name, IEnumerable<SelectListItem> items, bool nullOption = true, object htmlAttributes = null)
        {
            return Core.Mvc.ControllHelper.DropDownList(name, items, nullOption, htmlAttributes);
        }

        /// <summary>
        /// 将List数据包装为PageObj
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static PageObj<T> ToPageObj<T>(this CRL.LambdaQuery.LambdaQuery<T> query) where T : CRL.IModel, new()
        {
            var result = query.ToList();
            var pageObj = new PageObj<T>(result, query.SkipPage, query.RowCount, query.TakeNum);
            return pageObj;
        }

    }
}
