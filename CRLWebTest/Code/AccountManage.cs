using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTest.Code
{
    /// <summary>
    /// 帐户管理
    /// </summary>
    public class AccountManage : CRL.Package.Account.AccountBusiness<AccountManage>
    {
        public static AccountManage Instance
        {
            get { return new AccountManage(); }
        }

       
    }
}