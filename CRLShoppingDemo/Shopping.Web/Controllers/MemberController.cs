/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shopping.Model;
using Shopping.BLL;
namespace Shopping.Web.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberController : Core.Mvc.BaseController
    {
        //
        // GET: /Member/

        public ActionResult Index()
        {
            var userId = CurrentUser.Id;
            var account1 = BLL.Transaction.AccountManage.Instance.GetAccount(userId, Model.AccountType.会员, Model.TransactionType.现金);
            var account2 = BLL.Transaction.AccountManage.Instance.GetAccount(userId, Model.AccountType.会员, Model.TransactionType.积分);
            ViewBag.account1 = account1.AvailableBalance;
            ViewBag.account2 = account2.AvailableBalance;
            return View();
        }
        #region 登录
        [AllowAnonymous]
        public ActionResult Login(string returnUrl="")
        {
            if(returnUrl.ToLower().Contains("/supplier/"))
            {
                return RedirectToAction("Login", "Supplier", new { returnUrl = returnUrl });
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Model.Member member)
        {
            string error;
            var a = MemberManage.Instance.CheckPass(member.AccountNo, member.PassWord, out error);
            if (!a)
            {
                ModelState.AddModelError("", error);
                return View();
            }
            var u = MemberManage.Instance.QueryItem(b => b.AccountNo == member.AccountNo);
            if (u.Locked)
            {
                ModelState.AddModelError("", "账号已锁定");
                return View();
            }
            MemberManage.Instance.Login(u, "Member", false);
            string returnUrl = Request["returnUrl"];
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/";
            }
            return Redirect(returnUrl);
        }
        #endregion
        
        public ActionResult Charge(decimal amount)
        {
            var user=MemberManage.Instance.GetCurrent();
            string error;
            var a = MemberManage.Instance.Charge(user, amount, "前台充值", TransactionType.现金, out error);
            return RedirectToAction("index");
        }

        public ActionResult Transaction(TransactionType type = TransactionType.现金, int page = 1, int pageSize = 15)
        {
            ViewBag.TransactionType = type;
            var query = BLL.Transaction.TransactionManage.Instance.GetLambdaQuery();
            var accountId = BLL.Transaction.AccountManage.Instance.GetAccountId(CurrentUser.Id, AccountType.会员, type);
            query.Where(b => b.AccountId == accountId);

            var result = query.ToList();
            int count = query.RowCount;
            var pageObj = new PageObj<CRL.Package.Account.Transaction>(result, page, count, pageSize);
            return View(pageObj);
        }
    }
}
