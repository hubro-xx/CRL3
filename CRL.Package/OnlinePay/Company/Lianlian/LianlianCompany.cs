/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace CRL.Package.OnlinePay.Company.Lianlian
{
    public class LianlianCompany:CompanyBase
    {
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
            get { return CompanyType.连连; }
        }

        public override void Submit(PayHistory order)
        {
            base.BaseSubmit(order);
            var request = new Message.Web.PayRequest();
            request.version = PartnerConfig.VERSION;
            request.oid_partner = PartnerConfig.OID_PARTNER;
            request.user_id = order.UserId.ToString();
            request.timestamp = YinTongUtil.getCurrentDateTimeStr();
            request.sign_type = PartnerConfig.SIGN_TYPE;
            request.busi_partner = PartnerConfig.BUSI_PARTNER;
            request.no_order = order.OrderId;
            request.dt_order = YinTongUtil.getCurrentDateTimeStr();	
            request.name_goods = "在线充值";
            request.money_order = order.Amount.ToString();
            request.notify_url = notify_url;
            request.url_return = return_url;
            request.SetSign();
            var fields = request.GetType().GetFields();
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<form id='payBillForm' action='" + request.InterFaceUrl + "' method='post'>");

            foreach (var temp in fields)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Name + "' value='" + temp.GetValue(request) + "'/>");
            }
            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='tijiao' style='display:none;'></form>");
            sbHtml.Append("<script>document.forms['payBillForm'].submit();</script>");
            HttpContext.Current.Response.Write(sbHtml.ToString());
        }
        protected override string OnNotify(System.Web.HttpContext context)
        {
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string json = stream.ReadToEnd();
            stream.Close();
            var response = Message.MessageBase.FromRequest<Message.Web.PayNotify>(json);
            var a = response.CheckSign();
            if (a && response.result_pay == "SUCCESS")
            {
                PayHistory order = OnlinePayBusiness.Instance.GetOrder(response.no_order, ThisCompanyType);
                Confirm(order, GetType(), Convert.ToDecimal(response.money_order));
                var result = new { ret_code = "0000", ret_msg = "交易成功" };
                return CoreHelper.SerializeHelper.SerializerToJson(result);
            }
            var result2 = new { ret_code = "9999", ret_msg = "失败" };
            return CoreHelper.SerializeHelper.SerializerToJson(result2);
        }

        public bool GetReturn(System.Web.HttpContext context)
        {
            var response = Message.MessageBase.FromRequest<Message.Web.PayReturn>(context.Request.Form);
            var a = response.CheckSign();
            if (!a)
            {
                return false;
            }
            return response.result_pay == "SUCCESS";
        }
        public override bool CheckOrder(PayHistory order, out string message)
        {
            throw new NotImplementedException();
        }

        public override bool RefundOrder(PayHistory order, out string message)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// WAP认证支付
        /// 首次需传入银行卡进行绑定
        /// 再次按绑定的银行卡进行支付
        /// </summary>
        /// <param name="order"></param>
        /// <param name="id_no"></param>
        /// <param name="acct_name"></param>
        /// <param name="card_no"></param>
        /// <param name="no_agree"></param>
        /// <param name="risk_item"></param>
        public void WapAuthSubmit(PayHistory order, string id_no, string acct_name, string card_no, string no_agree, string risk_item)
        {
            var request = new Message.WapAuth.PayRequest();
            request.version = PartnerConfig.VERSION;
            request.oid_partner = PartnerConfig.OID_PARTNER;
            request.user_id = order.UserId.ToString();
            request.sign_type = PartnerConfig.SIGN_TYPE;
            request.busi_partner = PartnerConfig.BUSI_PARTNER;
            request.no_order = order.OrderId;
            request.dt_order = YinTongUtil.getCurrentDateTimeStr();
            request.name_goods = "在线充值";
            request.money_order = order.Amount.ToString();
            request.notify_url = CoreHelper.CustomSetting.GetConfigKey("lianlianWapAuthNotifyUrl");
            request.url_return = CoreHelper.CustomSetting.GetConfigKey("lianlianWapAuthReturnUrl");
            request.id_type = "0";
            request.id_no = id_no;
            request.acct_name = acct_name;
            request.card_no = card_no;
            request.no_agree = no_agree;
            request.risk_item = risk_item;//风控参数
            request.SetSign();
            var data = CoreHelper.SerializeHelper.SerializerToJson(request, Encoding.UTF8);
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<form id='payBillForm' action='" + request.InterFaceUrl + "' method='post'>");
            sbHtml.Append("<input name='req_data' value='" + data + "' /></form>");
            sbHtml.Append("<script>document.forms['payBillForm'].submit();</script>");
            System.Web.HttpContext.Current.Response.Write(sbHtml.ToString());
        }
        public string WapAuthNotify(System.Web.HttpContext context)
        {
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string json = stream.ReadToEnd();
            stream.Close();
            var response = Message.MessageBase.FromRequest<Message.WapAuth.PayNotify>(json);
            var a = response.CheckSign();
            if (a && response.result_pay == "SUCCESS")
            {
                PayHistory order = OnlinePayBusiness.Instance.GetOrder(response.no_order, ThisCompanyType);
                Confirm(order, GetType(), Convert.ToDecimal(response.money_order));
                var result = new { ret_code = "0000", ret_msg = "交易成功" };
                return CoreHelper.SerializeHelper.SerializerToJson(result);
            }
            var result2 = new { ret_code = "9999", ret_msg = "失败" };
            return CoreHelper.SerializeHelper.SerializerToJson(result2);
        }
        public bool GetWapAuthReturn(System.Web.HttpContext context)
        {
            var response = Message.MessageBase.FromRequest<Message.WapAuth.PayReturn>(context.Request.Form);
            var a = response.CheckSign();
            if (!a)
            {
                return false;
            }
            return response.result_pay == "SUCCESS";
        }
    }
}
