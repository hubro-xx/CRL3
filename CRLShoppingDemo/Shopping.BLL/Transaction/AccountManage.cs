
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CRL;
namespace Shopping.BLL.Transaction
{
    /// <summary>
    /// 交易帐户管理
    /// 给不类型账号分配不同的交易帐户
    /// </summary>
    public class AccountManage : CRL.Package.Account.AccountBusiness<AccountManage>
    {
        public static AccountManage Instance
        {
            get { return new AccountManage(); }
        }

    }
}
