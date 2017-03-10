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
using CRL;
namespace Core.Mvc
{
    public class ControllHelper
    {
        /// <summary>
        /// 页面请求验证码,每天换一次
        /// </summary>
        public static string RequestToken
        {
            get
            {
                return CoreHelper.StringHelper.EncryptMD5(DateTime.Now.ToString("yyMMddHH") + "RequestToken");
            }
        }
        /// <summary>
        /// 创建分页导航
        /// </summary>
        public static MvcHtmlString CreatePageNavigation<T>(PageObj<T> pageObj, CoreHelper.PageNavigation.PageStyle style = CoreHelper.PageNavigation.PageStyle.Google, string navFormat = "")
        {
            var nav = new CoreHelper.PageNavigation();
            nav.SetPageStyle(style);
            nav.PageNavigationFormat = navFormat;
            if (pageObj.Count == 0)
            {
                return MvcHtmlString.Create("");
            }
            string str = nav.GetPageNavigation(pageObj.PageIndex, pageObj.Total, pageObj.PageSize);
            return MvcHtmlString.Create(str);
        }
        
        
        /// <summary>
        /// 使用传入默认值,创建SelectListItem
        /// </summary>
        /// <param name="selectValue"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<SelectListItem> MakeSelectByValue(object selectValue, Type type, bool nullOption = true)
        {
            var list = new List<SelectListItem>();
            if (nullOption)
            {
                list.Add(new SelectListItem() { Value = "", Text = "全部" });
            }
            foreach (var item in Enum.GetValues(type))
            {
                string value = ((int)item).ToString();
                bool selected = false;
                if (selectValue + "" != "")
                {
                    selected = (int)item == Convert.ToInt32(selectValue);
                }
                list.Add(new SelectListItem() { Value = value, Text = item.ToString(), Selected = selected });
            }
            return list;
        }
        /// <summary>
        /// 按枚举创建已选择的DropDownList
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectValue"></param>
        /// <param name="nullOption"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownList(string name, Enum selectValue, bool nullOption = true, object htmlAttributes = null)
        {
            var list = MakeSelectByValue(selectValue, selectValue.GetType(), false);
            return DropDownList(name, list, nullOption, htmlAttributes);
        }
        /// <summary>
        /// 按request值创建已选择的DropDownList
        /// </summary>
        /// <param name="type"></param>
        /// <param name="requestName"></param>
        /// <param name="nullOption"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownList(Enum type, string requestName, bool nullOption = true, object htmlAttributes = null)
        {
            string requestValue = System.Web.HttpContext.Current.Request[requestName];
            var items = MakeSelectByValue(requestValue, type.GetType(), nullOption);
            return DropDownList(requestName, items, nullOption, htmlAttributes);
        }
        /// <summary>
        /// 自定义创建DropDownList
        /// </summary>
        /// <param name="name"></param>
        /// <param name="items"></param>
        /// <param name="nullOption"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownList(string name, IEnumerable<SelectListItem> items, bool nullOption = true, object htmlAttributes = null)
        {
            var html = string.Format("<select id='{0}' name='{0}' ", name);
            if (htmlAttributes != null)
            {
                var fileds = htmlAttributes.GetType().GetProperties();
                foreach (var f in fileds)
                {
                    html += string.Format("{0}=\"{1}\"", f.Name, f.GetValue(htmlAttributes, null));
                }
            }
            html += ">";
            foreach (var item in items)
            {
                html += string.Format("<option value='{0}'{1}>{2}</option>", item.Value, item.Selected ? " selected" : "", item.Text);
            }
            html += "</select>";
            return MvcHtmlString.Create(html);
        }
        /// <summary>
        /// 替换参数值
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ReplaceParame(NameValueCollection c1, string key, object value)
        {
            NameValueCollection c = new NameValueCollection(c1);
            c.Remove("ie");
            c.Remove(key);
            c.Add(key, value + "");
            string str = "";
            foreach (string k in c.Keys)
            {
                str += k + "=" + System.Web.HttpUtility.UrlEncode(c[k]) + "&";
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - 1);
            }
            return "?" + str;
        }
    }
    
}
