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
        protected override Type BusinessTypeForCheck()
        {
            return typeof(BLL.CartManage);
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Test()
        {
            return Content("test");
        }
        public ActionResult test2()
        {
            var query = BLL.MemberManage.Instance.GetLambdaQuery();
            query.Where(b => b.Id > 0);
            query.Page(15, 1);
            var result = query.ToPageObj();
            return Content("");
        }
        public ActionResult Logoff()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("index");
        }
    }
}
