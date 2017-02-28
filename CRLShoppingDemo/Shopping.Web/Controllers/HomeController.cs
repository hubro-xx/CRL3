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
using System.Web;
using System.Web.Mvc;
using Shopping.BLL;
using Core.Mvc;
namespace Shopping.Web.Controllers
{
    [Core.Mvc.MenuAuthor]
    public class HomeController : Core.Mvc.BaseController
    {
        protected override Type CRLModelTypeForCheck()
        {
            return typeof(BLL.CartManage);
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            //var a = 0;
            //int b = 10 / a;
            return View();
        }
        [AllowAnonymous]
        public ActionResult Test()
        {
            return Content("test");
        }

        public ActionResult Logoff()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("index");
        }
        #region 图片上传
        public ActionResult UploadImage()
        {
            return _UploadImageView(true, Request["folder"], Request["uploadType"]);
        }
        [HttpPost]
        public ActionResult UploadImage(string folder="test", string uploadType="test")
        {
            //要使用服务上传,配置uploadUseService为1
            //配置UploadServiceUrl地址为上传服务地址
            //CORE_UploadImagePath配置为网站止录或文件目录
            string html = "";
            var saveFiles = new List<string>();
            bool a = false;
            string message = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file.ContentLength == 0)
                    continue;
                string saveFile;
                a = _UploadImage(file, folder, out saveFile, out message);
                if (a)
                {
                    saveFiles.Add(saveFile);
                    if (uploadType == "product")
                    {
                        //在这里根据类型生成缩略图
                        MakeThumbImage(saveFile, 100100, 150150, 200200, 250250, 350350, 450450);
                    }
                }
            }
            if (a)
            {
                html = "<script>opener.OnFileUpload('" + string.Join(",", saveFiles) + "');window.close()</script>";
            }
            else
            {
                html = "<script>alert('上传失败:" + message + "');window.history.back()</script>";
            }
            return Content(html);
        }
        #endregion
    }
}
