using Shopping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shopping.BLL;
namespace Shopping.Web.Controllers
{
    public class HomeController : Core.Mvc.BaseController
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult Logoff()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("index");
        }
    }
}
