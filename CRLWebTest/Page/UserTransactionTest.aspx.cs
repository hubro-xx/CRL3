/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CRL;
namespace WebTest
{
    public partial class UserTransactionTest : System.Web.UI.Page
    {
        public List<CRL.Package.Account.Transaction> data;
        int accountType = 0;//可扩展,帐号类型 ,会员?商家
        int transactionType = 0;//可扩展,账户类型 钱?积分?优惠券
        protected void Page_Load(object sender, EventArgs e)
        {
            
            Bind();
        }
        void Bind()
        {
            var account = Code.AccountManage.Instance.GetAccount(TextBox1.Text.ToInt(), accountType, transactionType);
            var query = Code.TransactionManage.Instance.GetLambdaQuery();
            data = query.ToList();
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            var account = Code.AccountManage.Instance.GetAccount(TextBox1.Text.ToInt(), accountType, transactionType);
            Response.Write("帐户余额为:" + account.CurrentBalance);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var account = Code.AccountManage.Instance.GetAccount(TextBox1.Text.ToInt(), accountType, transactionType);
            decimal amount=Convert.ToInt32(TextBox2.Text);
            int op = Convert.ToInt32(drpOperate.SelectedValue);
            //创建交易流水
            var ts = new CRL.Package.Account.Transaction() { AccountId = account.Id, Amount = amount, OperateType = (CRL.Package.Account.OperateType)op, TradeType = op, Remark = "业务交易:" + amount };
            ts.OutOrderId = DateTime.Now.ToShortTimeString();//外部订单号,会用来判断有没有重复提交
            string error;
            //提交交易
            bool a = Code.TransactionManage.Instance.SubmitTransaction(out error, true, ts);
            Response.Write("操作" + a + " " + error);
            Bind();
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            var account = Code.AccountManage.Instance.GetAccountId(TextBox1.Text.ToInt(), accountType, transactionType);
            var lockRecord = new CRL.Package.Account.LockRecord() { AccountId = account, Amount = 1, Remark = "sfsdf" };
            string error;
            var a = Code.TransactionManage.Instance.LockAmount(lockRecord, out error);
            txtLockId.Text = lockRecord.Id.ToString();
            Response.Write(a + " " + lockRecord.Id + " " + error);
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            string error;
            var lockId = Convert.ToInt32(txtLockId.Text);
            var a = Code.TransactionManage.Instance.UnlockAmount(lockId, out error);
            Response.Write(a + " " + error);
        }
    }
}
