using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Xml;
using System.IO;
namespace CRL.Package.OnlinePay.Company.Weixin
{
    public class WeixinCompany:CompanyBase
    {
        

        public override CompanyType ThisCompanyType
        {
            get { return CompanyType.微信; }
        }
        /// <summary>
        /// 获取OPENID,输出wxEditAddrParam到页面
        /// 页面执行脚本调用微信
        /// </summary>
        /// <param name="context"></param>
        /// <param name="wxEditAddrParam"></param>
        /// <returns></returns>
        public string GetOpenId(System.Web.HttpContext context, out string wxEditAddrParam)
        {
            JsApiPay jsApiPay = new JsApiPay(context);
            //调用【网页授权获取用户信息】接口获取用户的openid和access_token
            jsApiPay.GetOpenidAndAccessToken();

            //获取收货地址js函数入口参数
            wxEditAddrParam = jsApiPay.GetEditAddressParameters();
            return jsApiPay.openid;

        }
        public override void Submit(PayHistory order)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 开始支付,输出到页面wxJsApiParam
        /// 页面执行脚本调用微信
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public string BeginPay(PayHistory order)
        {
            BaseSubmit(order);
            JsApiPay jsApiPay = new JsApiPay(System.Web.HttpContext.Current);
            jsApiPay.openid = order.TagData;//传入OPENID
            jsApiPay.total_fee = Convert.ToInt32(order.Amount * 100);
            //JSAPI支付预处理
            WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult(order);
            var wxJsApiParam = jsApiPay.GetJsApiParameters();//获取H5调起JS API参数                    
            return wxJsApiParam;
        }

        protected override string OnNotify(System.Web.HttpContext context)
        {
            ResultNotify resultNotify = new ResultNotify(context);
            string error;
            string out_trade_no;
            var a = resultNotify.ProcessNotify(out error, out out_trade_no);
            if (a)
            {
                PayHistory order = OnlinePayBusiness.Instance.GetOrder(out_trade_no, ThisCompanyType);
                Confirm(order, GetType(), order.Amount);
            }
            return "";
        }

        public override bool CheckOrder(PayHistory order, out string message)
        {
            var result = OrderQuery.Run("", order.OrderId, out message);//调用订单查询业务逻辑
            if (result)
            {
                Confirm(order, GetType(), order.Amount);
            }
            return result;
        }

        public override bool RefundOrder(PayHistory order, out string message)
        {
            throw new NotImplementedException();
        }
    }
}
