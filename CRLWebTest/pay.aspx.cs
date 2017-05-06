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
using CRL.Package;
using CRL;
namespace WebTest
{
    public partial class pay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var request = new CRL.Package.OnlinePay.Company.Bill99.GetDynNumContent.Request();
            request.bankId = "ICBC";
            request.customerId = "201105";
            request.amount = "100.00";//必填

            //以下第二次鉴权可以不需要
            request.cardHolderName = "测试";
            request.idType = "0";
            request.cardHolderId = "340827198512011810";
            request.pan = "4380880000000007";//卡号
            request.expiredDate = "0911";
            request.phoneNO = "15861806195";
            request.cvv2 = "111";

            var result = CRL.Package.OnlinePay.Company.Bill99.Bill99Util.PCIStore(request, true, out token);
            Response.Write(string.Format("{0},{1}",result,token));
        }
        static string token;
        protected void Button2_Click(object sender, EventArgs e)
        {
            var data = new Code.ProductData();
            data.Id = 1;
            string remark = System.IO.File.ReadAllText(Server.MapPath("code.txt"));
            data.Change(b => b.Remark, remark);
            Code.ProductDataManage.Instance.Update(data);
        }
    }
}
