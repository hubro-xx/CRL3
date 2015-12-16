using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Core.Mvc
{
    public class BaseController : System.Web.Mvc.Controller
    {
        public class PageObj<T> : Core.Mvc.PageObj<T>
        {
            public PageObj(IEnumerable<T> allItems, int pageIndex, int total, int pageSize)
                : base(allItems, pageIndex, total, pageSize)
            {
                
            }
        }
        public static int CurrentSystemId
        {
            get
            {
                if (!CoreHelper.CustomSetting.ContainsKey("CurrentSystemId"))
                {
                    return 1;
                }
                return Convert.ToInt32(CoreHelper.CustomSetting.GetConfigKey("CurrentSystemId"));
            }
        }
        #region DealResult
        /// <summary>
        /// 处理结果
        /// </summary>
        public class DealResult
        {
            /// <summary>
            /// 处理结果
            /// </summary>
            public bool Result
            {
                get;
                set;
            }
            /// <summary>
            /// 消息
            /// </summary>
            public string Message
            {
                get;
                set;
            }
            /// <summary>
            /// 关联数据
            /// </summary>
            public object Data
            {
                get;
                set;
            }
        }
        #endregion

        /// <summary>
        /// 当前登录用户
        /// </summary>
        public CRL.Package.Person.Person CurrentUser
        {
            get
            {
                return CRL.Package.RoleAuthorize.EmployeeBusiness.CurrentUser;
            }
        }

        /// <summary>
        /// 检测重复提交表单
        /// </summary>
        /// <param name="tokenName"></param>
        public void CheckDuplicateSubmit(string tokenName = "PageToken")
        {
            var key = Request[tokenName];
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            string concurrentKey = "submitRepeatedCheck_" + key;
            if (!CoreHelper.ConcurrentControl.Check(concurrentKey, 10))
            {
                throw new Exception("请不要重复提交表单");
            }
        }
        #region 访问权限控制
        /// <summary>
        /// 检测菜单访问权限
        /// </summary>
        /// <returns></returns>
        public bool CheckAccess()
        {
            bool a = CRL.Package.RoleAuthorize.AccessControlBusiness.Instance.CheckAccess(CurrentSystemId, CurrentUser.Id);
            return a;
        }
        /// <summary>
        /// 返回没有权限提示
        /// </summary>
        /// <returns></returns>
        public System.Web.Mvc.ActionResult NotAuthorityResult(bool json = false)
        {
            var menu = CRL.Package.RoleAuthorize.MenuBusiness.Instance.GetMenuByUrl(CurrentSystemId);
            string message;
            if (menu == null)
            {
                message = string.Format("没有权限进行此操作,请联系管理员分配权限 \n地址:[{0}]",Request.Path);
            }
            else
            {
                message = string.Format("没有权限进行此操作,请联系管理员分配权限 \n菜单:[{0}] 地址:[{1}]", menu.Name, menu.Url);
            }
            if (json)
            {
                return JsonResult(false, message);
            }
            return PageContent("没有权限", message);
        }
        #endregion
        /// <summary>
        /// 自动返回上一页
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public System.Web.Mvc.ActionResult AutoBackResult(string message="")
        {
            return AutoBackResult(message, "");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reloadParent">ture刷新父页面，false不刷新</param>
        /// <returns></returns>
        protected System.Web.Mvc.ActionResult CloseFrameResult(bool reloadParent=true)
        {
            if (reloadParent)
            {
                return PageContent("操作成功", "2秒后窗口自动关闭<script>setTimeout('parent.$.fancybox.close()', 2000);setTimeout('parent.location.reload()', 2000);</script>");
            }
            else
            {
                return PageContent("操作成功", "2秒后窗口自动关闭<script>setTimeout('parent.$.fancybox.close()', 2000)</script>");
            }
        }
        /// <summary>
        /// 自动返回到指定URL
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public System.Web.Mvc.ActionResult AutoBackResult(string message, string url)
        {
            if (string.IsNullOrEmpty(message))
                message = "操作成功";
            string backJs = "window.location.href ='" + url + @"';";
            string errorMsg = "失败,不正确,不成功,出错,错误,不对,终止,异常";
            var errorArry = errorMsg.Split(',');
            bool findError = false;
            foreach (var s in errorArry)
            {
                if (message.Contains(s))
                {
                    findError = true;
                    break;
                }
            }
            string html = "";
            if (string.IsNullOrEmpty(url))//URL为空则后退
            {
                backJs = "window.history.back();";
            }
            html = @"<script type='text/javascript' reload='1'>
function goUrl()
{
    " + backJs + @"
}
</script>";
            if (!findError)
            {
                html += @"
<script type='text/javascript' reload='1'>
setTimeout('goUrl()', 3300)</script>";
            }
            html += @"
<a href='javascript:goUrl()'>如果您的浏览器没有自动跳转，请点击此链接</a>";
            //html = string.Format(html, url, message);
            return PageContent(message, html);
        }
        /// <summary>
        /// 根据METHOD判断跳转还是返回TRUE
        /// </summary>
        /// <returns></returns>
        public System.Web.Mvc.ActionResult AutoReturn(string message)
        {
            if (!Request.IsAjaxRequest())
            {
                return AutoBackResult(message, Request.UrlReferrer + "");
            }
            else
            {
                return JsonResult(true, message);
            }
        }
        /// <summary>
        /// 输出继承布局的Content
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ActionResult PageContent(string title, string content)
        {
            var result = new CustomActionResult("~/views/shared/PageContent.cshtml");
            TempData["content"] = content;
            TempData["title"] = title;
            result.TempData = TempData;
            return result;
        }
        public ActionResult PageContent(string title)
        {
            return PageContent(title, "<a href='javascript:history.back()'>返回上一页</a>");
        }
        /// <summary>
        /// 返回封装好的Json结果
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public System.Web.Mvc.JsonResult JsonResult(bool result, string message, object data)
        {
            DealResult obj = new DealResult() { Result = result, Message = message, Data = data };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 返回封装好的Json结果
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public System.Web.Mvc.JsonResult JsonResult(bool result, string message)
        {
            return JsonResult(result, message, "");
        }

        protected ActionResult DecryptImg(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                //Response.StatusCode = 500;
                return Content("缺少参数");
            }
            string path = file.Replace("/", @"\");
            if (!path.StartsWith(@"\"))
            {
                path = @"\" + path;
            }
            string path2 = CoreHelper.ImageUpload.Upload.BaseFolder + path;
            System.Drawing.Image bmp = null;
            try
            {
                bmp = CoreHelper.ImageUpload.FileEncrypt.DecryptImg(path2);
            }
            catch
            {
                //Response.StatusCode = 404;
                return Content("没找到文件");
            }
            Bitmap bmp2 = new Bitmap(bmp);
            MemoryStream ms = new MemoryStream();
            try
            {
                bmp2.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                Response.ClearContent();
                Response.ContentType = "image/Jpeg";
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ero)
            {
                //Response.StatusCode = 500;
                return Content(ero.Message);
            }
            finally
            {
                bmp.Dispose();
                //显式释放资源
                bmp2.Dispose();
                ms.Dispose();
            }
            return null;
        }

    }
}