using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTest.Code
{
    /// <summary>
    /// 交易流水管理
    /// </summary>
    public class TransactionManage : CRL.Package.Account.TransactionBusiness<TransactionManage>
    {
        public static TransactionManage Instance
        {
            get { return new TransactionManage(); }
        }

    }
}