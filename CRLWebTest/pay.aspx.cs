using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CRL.Package;
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
            var request = new CRL.Package.OnlinePay.Company.Bill99.TxnMsgContent.Request();
            request.amount = "100.00"; //与鉴权订单金额一致
            request.customerId = "201105";//必须要

            var ex = new CRL.Package.OnlinePay.Company.Bill99.TxnMsgContent.Request._extData();
            //扩展字段信息        
            ex.validCode = "949378";//手机验证码
            ex.savePciFlag = "1";//是否保存鉴权信息 1保存 0不保存
            ex.token = token;//手机验证令牌
            ex.payBatch = "2";//快捷支付批次 1首次支付 2再次支付
            ex.phone = "15861806195";
            request.extData = ex;

            //以下第二次支付可以不用填写
            request.cardNo = "4380880000000007";
            request.expiredDate = "0911";
            request.cvv2 = "111";
            request.cardHolderName = "测试";
            request.cardHolderId = "340827198512011810";
            request.idType = "0";
            string error;
            var result = CRL.Package.OnlinePay.Company.Bill99.Bill99Util.Purchase(request, true, out error);
            Response.Write(string.Format("{0},{1}", result, error));
        }
    }
}