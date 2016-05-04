/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
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
        #region 自定义Result
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
        #endregion

        #region 图片上传
        protected ActionResult _UploadImageView(bool multiple, string folder, string uploadType)
        {
            string html = Properties.Resources.upload;
            html = html.Replace("[addFile]", multiple ? "<input type='button' onclick='add()' value='增加' />" : "");
            html = html.Replace("[folder]", folder);
            html = html.Replace("[uploadType]", uploadType);
            return Content(html);
        }
        /// <summary>
        /// 上传图片方法
        /// 使用服务需配置服务地址
        /// </summary>
        /// <param name="postFile"></param>
        /// <param name="folder"></param>
        /// <param name="saveFile"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected bool _UploadImage(HttpPostedFileBase postFile, string folder,out string saveFile, out string error)
        {
            var useService = CoreHelper.CustomSetting.GetConfigKey("uploadUseService") == "1";
            //string saveFile;
            bool a;
            var stream = postFile.InputStream;
            string fileName = postFile.FileName;
            if (useService)
            {
                var u = new CoreHelper.ImageUpload.UploadService();
                a = u.UploadFile(stream, fileName, out saveFile, out error);
            }
            else
            {
                a = CoreHelper.ImageUpload.Upload.CheckFile(stream, postFile.FileName, out error);
                if (!a)
                {
                    saveFile = "";
                    return false;
                }
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                a = CoreHelper.ImageUpload.Upload.SaveFile(data, folder, 0, out error, out saveFile);
            }
            return a;
        }
        /// <summary>
        /// 生成缩略图方法
        /// </summary>
        /// <param name="file"></param>
        /// <param name="thumbnailMode"></param>
        protected void MakeThumbImage(string file, params int[] thumbnailMode)
        {
            var useService = CoreHelper.CustomSetting.GetConfigKey("uploadUseService") == "1";
            if (useService)
            {
                var u = new CoreHelper.ImageUpload.UploadService();
                u.MakeThumbImage(file, thumbnailMode);
            }
            else
            {
                CoreHelper.ImageUpload.Upload.MakeThumbImage(file, thumbnailMode);
            }
        }
        #endregion

        #region 控制台
        /// <summary>
        /// 缓存管理
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult _CacheManage(string key = "")
        {
            string html = "<h2>缓存管理</h2>";
            html += "<a href='_ModelManage'>对象管理</a>";
            if (!string.IsNullOrEmpty(key))
            {
                var a = CRL.MemoryDataCache.CacheService.UpdateCache(key);
                html += "<h4 style='color:red'>更新缓存" + a + " " + DateTime.Now + "</h4>";
            }
            var caches = CRL.MemoryDataCache.CacheService.GetCacheList();
            html += @"
<table border='1' style='width:100%'>
    <tr>
        <td style='width:40px'>操作</td>
        <td>KEY</td>
        <td>数据类型</td>
        <td>行数</td>
        <td>过期(分)</td>
        <td>上次更新</td>
        <td style='width:200px'>查询</td>
        <td style='width:100px'>参数</td>

    </tr>";
            foreach (var item in caches)
            {
                string part = @"<tr>
                <td><a href='?key={0}'>更新</a></td>
        <td>{0}</td>
        <td>{1}</td>
        <td>{2}</td>
        <td>{3}</td>
        <td>{4}</td>
        <td>{5}</td>
        <td>{6}</td>

    </tr>";
                part = string.Format(part, item.Key, item.DataType,item.RowCount, item.TimeOut, item.UpdateTime, item.TableName, item.Params);
                html += part;
            }

            html += "</table>";

            return Content(html);
        }
        /// <summary>
        /// 重写以获取结构检查对象程序集
        /// </summary>
        /// <returns></returns>
        protected virtual Type BusinessTypeForCheck()
        {
            return null;
        }
        public ActionResult _ModelManage(string chkType="")
        {
            var currentType = BusinessTypeForCheck();
            string html = @"<h2>对象管理</h2>";
            html += "<a href='_CacheManage'>缓存管理</a>";
            if (currentType == null)
            {
                return Content("请重写BusinessTypeForCheck以获取反射的MODEL");
            }
            var findTypes = CRL.Base.GetAllModel(currentType);
            if (!string.IsNullOrEmpty(chkType))
            {
                var find = findTypes.Keys.Where(b => b.Type.FullName == chkType).FirstOrDefault();
                if (find != null)
                {
                    var db = findTypes[find];
                    object obj = System.Activator.CreateInstance(find.Type);
                    CRL.IModel b = obj as CRL.IModel;
                    var msg = b.CreateTable(db);
                    html += "<h4 style='color:red'>检查结构" + find.Type + "" + (string.IsNullOrEmpty(msg) ? "成功" : msg) + "</h4>";
                }
            }
            html += @"
<table border='1' style=''>
    <tr>
        <td>名称</td>
        <td>表名</td>
        <td>主键</td>
        <td width='40'>操作</td>
    </tr>";
            foreach (var kv in findTypes)
            {
                var item = kv.Key;
                string part = @"<tr>
        <td>{0}</td>
        <td>{1}</td>
<td>{2}</td>
        <td><a href='?chkType={0}'>检查结构</a></td>
    </tr>";
                var primaryKey = item.GetPrimaryKey();

                part = string.Format(part, item.Type.FullName, item.TableName, primaryKey.GetName() + "(" + primaryKey.GetPropertyType() + ")");
                html += part;
            }
            return Content(html);
        }
        #endregion

        #region 自定义错误
        static Exception GetInnerException(Exception exp)
        {
            if (exp.InnerException != null)
            {
                exp = exp.InnerException;
                return GetInnerException(exp);
            }
            return exp;
        }

        public ActionResult _CustomError()
        { 
            var ero = RouteData.Values["httpException"] as Exception;
            if (Request.IsAjaxRequest())
            {
                return JsonResult(false, "服务器内部错误:" + ero.Message);
            }
            var errorCode = RouteData.Values["errorCode"] as String;
            string html = Properties.Resources.erro;
            html = html.Replace("[charset]", Request.ContentEncoding.WebName);
            ero = GetInnerException(ero);
            if (ero != null)
            {
                string erroDetail = ero.StackTrace + "";
                erroDetail = erroDetail.Replace("\r\n", "<br>");
                erroDetail = Server.HtmlEncode(erroDetail);
                html = html.Replace("[TIME]", DateTime.Now.ToString());
                html = html.Replace("[ERRO_CODE]", errorCode);
                html = html.Replace("[URL]", Request.Url.ToString());
                html = html.Replace("[ERRO_TITLE]", Server.HtmlEncode(ero.Message));
                html = html.Replace("[ERRO_MESSAGE]", erroDetail);
            }

            return Content(html);
        }
        #endregion
    }
}
