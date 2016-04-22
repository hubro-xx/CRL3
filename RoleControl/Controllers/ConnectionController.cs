using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RoleControl.Controllers
{
    [Authorize]
    public class ConnectionController : RoleControl.Models.BaseController
    {
        //
        // GET: /Connection/

        public ActionResult Index()
        {
            var connections = ConnectionManage.GetConnections();
            ViewBag.connections = string.Join("\r\n", connections);
            return View();
        }

        [HttpPost]
        public ActionResult Index(string txt)
        {
            var arry = txt.Split(new char[]{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
            ConnectionManage.cache = arry.ToList();
            ConnectionManage.Save();
            ViewBag.connections = txt;
            return View();
        }
        public ActionResult Change(int index,string returnUrl)
        {
            ConnectionManage.index = index;
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/";
            }
            return Redirect(returnUrl);
        }
    }
}
