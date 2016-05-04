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

namespace CRL.Package.OnlinePay.Company.Chinapnr
{
    public class ChinapnrCompany:CompanyBase
    {
        static Encoding encode = Encoding.GetEncoding("gb2312");
        public override CompanyType ThisCompanyType
        {
            get { return CompanyType.汇付天下; }
        }
        public override void Submit(PayHistory order)
        {
            BaseSubmit(order);
            var request = new ChargeSubmit();
            request.MerId = MerchantId;
            request.CmdId = "Buy";
            request.OrdId = order.OrderId;
            request.OrdAmt = order.Amount.ToString("f");
            request.BgRetUrl = NotifyUrl;
            request.RetUrl = ReturnUrl;
            request.ChkValue = request.MakeSign(MerchantId);
            var fields = request.GetType().GetFields();
            //测试
            string html = "<form id='form1' name='form1' action='https://mas.chinapnr.com/gar/RecvMerchant.do' method='post'>\r\n";
            foreach (var item in fields)
            {
                html += string.Format("<input type='hidden' Name='{0}' value='{1}' />\r\n", item.Name, item.GetValue(request));
            }
            html += "</form>\r\n";
            html += "<script>form1.submit()</script>";
            HttpContext.Current.Response.Write(html);
        }
        protected override string OnNotify(System.Web.HttpContext context)
        {
            context.Request.ContentEncoding = encode;
            var c = context.Request.Form;
            var fields = typeof(ChargeResponse).GetFields();
            var obj = new ChargeResponse();
            foreach (var item in fields)
            {
                item.SetValue(obj, c[item.Name]);
            }
            if (!obj.CheckSign(obj.ChkValue))
            {
                return "签名不正确";
            }
            if (obj.RespCode == "000000")
            {
                PayHistory order = OnlinePayBusiness.Instance.GetOrder(obj.OrdId, ThisCompanyType);
                Confirm(order, GetType(), Convert.ToDecimal(obj.OrdAmt));
                return string.Format("RECV_ORD_ID_{0}", obj.OrdId);
            }
            return string.Format("失败 {0}", obj.RespCode);
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
