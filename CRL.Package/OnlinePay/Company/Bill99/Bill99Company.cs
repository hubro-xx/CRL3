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
using System.Text;
using System.Web;

namespace CRL.Package.OnlinePay.Company.Bill99
{
    public class Bill99Company : CompanyBase
    {
        static string merchantAcctId = ChargeConfig.GetConfigKey(CompanyType.快钱, ChargeConfig.DataType.User);
        string return_url
        {
            get
            {
                return GetAbsUrl(ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.ReturnUrl));
            }
        }
        string notify_url
        {
            get
            {
                return GetAbsUrl(ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.NotifyUrl));
            }
        }
        public override CompanyType ThisCompanyType
        {
            get { return CompanyType.快钱; }
        }
        public override void Submit(PayHistory order)
        {
            BaseSubmit(order);
            var request = new ChargeRequest();
            request.merchantAcctId = merchantAcctId;
            request.payerName = "";
            request.payerContactType = "1";
            request.orderId = order.OrderId;
            request.orderAmount = (Convert.ToInt32(order.Amount) * 100).ToString();
            request.orderTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            request.productName = "在线支付";
            request.productNum = "1";
            request.productDesc = "在线支付";
            request.payType = "00";
            request.ext1 = order.UserId.ToString();
            request.bgUrl = notify_url;
            request.MakeSign();
            var fields = request.GetType().GetProperties();
            //测试
            string html = "<form id='form1' name='form1' action='https://sandbox.99bill.com/gateway/recvMerchantInfoAction.htm' method='post'>\r\n";
            foreach (var item in fields)
            {
                html += string.Format("<input type='hidden' Name='{0}' value='{1}' />\r\n", item.Name, item.GetValue(request, null));
            }
            html += "</form>\r\n";
            html += "<script>form1.submit()</script>";
            //Log(html);
            HttpContext.Current.Response.Write(html);
        }
        protected override string OnNotify(HttpContext context)
        {
            var response = ChargeNotify.FromRequest(context.Request.QueryString);
            var a = response.CheckSign();
            int result = 1;
            string msg1 = "error";
            if (a && response.payResult == "10")
            {
                PayHistory order = OnlinePayBusiness.Instance.GetOrder(response.orderId, ThisCompanyType);
                Confirm(order, GetType(), Convert.ToDecimal(response.payAmount) / 100);
                result = 1;
                msg1 = "success";
            }
            return "<result>" + result + "</result><redirecturl>" + return_url + "?msg=" + msg1 + "</redirecturl>";
        }
        public override bool CheckOrder(PayHistory order, out string message)
        {
            throw new NotImplementedException();
        }

        public override bool RefundOrder(PayHistory order, out string message)
        {
            throw new NotImplementedException();
        }
    }
}
