using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CRL;
namespace Shopping.BLL.Transaction
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
