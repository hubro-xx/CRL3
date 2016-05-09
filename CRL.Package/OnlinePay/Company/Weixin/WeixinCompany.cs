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
using Newtonsoft.Json;
using System.Xml;
using System.IO;
namespace CRL.Package.OnlinePay.Company.Weixin
{
    public class WeixinCompany:CompanyBase
    {
        //from https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=11_1

        public override CompanyType ThisCompanyType
        {
            get { return CompanyType.微信; }
        }
        /// <summary>
        /// 获取OPENID,输出wxEditAddrParam到页面
        /// 微信需要产生两次跳转,首次返回为空
        /// </summary>
        /// <param name="context"></param>
        /// <param name="wxEditAddrParam"></param>
        /// <returns></returns>
        public string GetOpenId(System.Web.HttpContext context, out string wxEditAddrParam)
        {
            JsApiPay jsApiPay = new JsApiPay(context);
            //调用【网页授权获取用户信息】接口获取用户的openid和access_token
            string jumpUrl;
            jsApiPay.GetOpenidAndAccessToken(out jumpUrl);
            if (string.IsNullOrEmpty(jumpUrl))
            {
                //获取收货地址js函数入口参数
                wxEditAddrParam = jsApiPay.GetEditAddressParameters();
                return jsApiPay.openid;
            }
            else
            {
                wxEditAddrParam = "";
                context.Response.Redirect(jumpUrl);
                return "";
            }

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
        /// <param name="openId"></param>
        /// <returns></returns>
        public string BeginPay(PayHistory order,string openId)
        {
            BaseSubmit(order);
            JsApiPay jsApiPay = new JsApiPay(System.Web.HttpContext.Current);
            jsApiPay.openid = openId;
            if (string.IsNullOrEmpty(jsApiPay.openid))
            {
                throw new Exception("openid为空,请传入 openId");
            }
            jsApiPay.total_fee = Convert.ToInt32(order.Amount * 100);
            //JSAPI支付预处理
            WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult(order);
            var wxJsApiParam = jsApiPay.GetJsApiParameters();//获取H5调起JS API参数                    
            return wxJsApiParam;
        }
        /// <summary>
        /// 扫码支付,返回二维码支付链接
        /// 按此链接生成二维码
        /// </summary>
        /// <param name="order"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public string GetNativePayUrl(PayHistory order, string openId)
        {
            BaseSubmit(order);
            JsApiPay jsApiPay = new JsApiPay(System.Web.HttpContext.Current);
            jsApiPay.openid = openId;
            jsApiPay.total_fee = Convert.ToInt32(order.Amount * 100);
            //JSAPI支付预处理
            WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult(order);
            //var wxJsApiParam = jsApiPay.GetJsApiParameters();//获取H5调起JS API参数                    
            //return wxJsApiParam;
            return unifiedOrderResult.GetValue("code_url").ToString();
        }

        protected override string OnNotify(System.Web.HttpContext context)
        {
            ResultNotify resultNotify = new ResultNotify(context);

            var result = resultNotify.ProcessNotify();
            var a = result.GetValue("return_code").ToString() == "SUCCESS";
            if (a)
            {
                var out_trade_no = result.GetValue("out_trade_no").ToString();
                PayHistory order = OnlinePayBusiness.Instance.GetOrder(out_trade_no, ThisCompanyType);
                Confirm(order, GetType(), order.Amount);
            }
            return result.ToXml(); ;
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
