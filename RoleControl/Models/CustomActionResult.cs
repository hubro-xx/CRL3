/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RoleControl.Models
{
    public class CustomActionResult : ActionResult
    {
        private string _masterName;
        private TempDataDictionary _tempData;
        private ViewDataDictionary _viewData;
        private System.Web.Mvc.ViewEngineCollection _viewEngineCollection;
        private string _viewName;
        public CustomActionResult(string viewName)
        {
            ViewName = viewName;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (string.IsNullOrEmpty(this.ViewName))
            {
                this.ViewName = context.RouteData.GetRequiredString("action");
            }
            ViewEngineResult result = null;
            if (this.View == null)
            {
                result = this.FindView(context);
                this.View = result.View;
            }
            //context.HttpContext.Response.Clear();
            //context.HttpContext.Response.StatusCode = 0x194;
            //context.HttpContext.Response.TrySkipIisCustomErrors = true;
            TextWriter output = context.HttpContext.Response.Output;
            ViewContext viewContext = new ViewContext(context, this.View, this.ViewData, this.TempData, output);
            this.View.Render(viewContext, output);
            if (result != null)
            {
                result.ViewEngine.ReleaseView(context, this.View);
            }
        }

        protected ViewEngineResult FindView(ControllerContext context)
        {
            ViewEngineResult result = this.ViewEngineCollection.FindView(context, this.ViewName, this.MasterName);
            if (result.View != null)
            {
                return result;
            }
            StringBuilder builder = new StringBuilder();
            foreach (string str in result.SearchedLocations)
            {
                builder.AppendLine();
                builder.Append(str);
            }
            throw new InvalidOperationException(builder.ToString());
        }

        public string MasterName
        {
            get
            {
                return (this._masterName ?? string.Empty);
            }
            set
            {
                this._masterName = value;
            }
        }

        public object Model
        {
            get
            {
                return this.ViewData.Model;
            }
        }

        public TempDataDictionary TempData
        {
            get
            {
                if (this._tempData == null)
                {
                    this._tempData = new TempDataDictionary();
                }
                return this._tempData;
            }
            set
            {
                this._tempData = value;
            }
        }

        public IView View { get; set; }

        public ViewDataDictionary ViewData
        {
            get
            {
                if (this._viewData == null)
                {
                    this._viewData = new ViewDataDictionary();
                }
                return this._viewData;
            }
            set
            {
                this._viewData = value;
            }
        }

        public System.Web.Mvc.ViewEngineCollection ViewEngineCollection
        {
            get
            {
                return (this._viewEngineCollection ?? ViewEngines.Engines);
            }
            set
            {
                this._viewEngineCollection = value;
            }
        }

        public string ViewName
        {
            get
            {
                return (this._viewName ?? "~/views/shared/error.404.cshtml");
            }
            set
            {
                this._viewName = value;
            }
        }
    }
}
