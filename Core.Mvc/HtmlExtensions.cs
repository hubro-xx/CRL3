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
    public static class HtmlExtensions
    {
        public static MvcHtmlString Pager<T>(this HtmlHelper html, PageObj<T> pageObj, CoreHelper.PageNavigation.PageStyle style = CoreHelper.PageNavigation.PageStyle.Google, string navFormat = "")
        {
            return Core.Mvc.HtmlHelper.CreatePageNavigation(pageObj, style, navFormat);
        }
    }
}
