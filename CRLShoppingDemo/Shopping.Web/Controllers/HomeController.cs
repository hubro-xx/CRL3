using Shopping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shopping.BLL;
namespace Shopping.Web.Controllers
{
    [Core.Mvc.MenuAuthor]
    public class HomeController : Core.Mvc.BaseController
    {
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
        public ActionResult Logoff()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("index");
        }
    }
}
