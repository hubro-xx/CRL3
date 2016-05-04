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
using System.Data;
using System.Web;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Xml;
using CoreHelper;
using tenpay;
namespace CRL.Package.OnlinePay.Company.Tenpay
{
    public class TenpayCompany : CompanyBase
    {

        //设置并发锁
        private Object thisLock = new Object();

        public override CompanyType ThisCompanyType
        {
            get { return CompanyType.财付通; }
        }
		public TenpayCompany()
		{
            key = ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.Key);
		}
        #region 参数

        string date = DateTime.Now.ToString("yyyyMMdd");
        //商户订单号
        string sp_billno = "" + DateTime.Now.ToString("HHmmss") + TenpayUtil.BuildRandomStr(4);

        /// <summary>
        /// 商户号
        /// </summary>
        string bargainor_id
        {
            get
            {
                return ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.User);
            }
        }
        /// <summary>
        /// 商户密钥
        /// </summary>
		string key = "";
        string return_url
        {
            get
            {
                return CurrentHost + ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.ReturnUrl);
            }
        }
        string notify_url
        {
            get
            {
                return CurrentHost + ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.NotifyUrl);
            }
        }
        string ip = HttpContext.Current.Request.UserHostAddress;

        #endregion

        #region 方法

        public override void Submit(PayHistory order)
        {
            base.BaseSubmit(order);

            var tenpay = new RequestHandler(HttpContext.Current);
            tenpay.init();
            tenpay.setKey(key);
            tenpay.setGateUrl("https://gw.tenpay.com/gateway/pay.htm");
            tenpay.setParameter("total_fee", ((int)(order.Amount * 100)).ToString());
            //用户的公网ip,测试时填写127.0.0.1,只能支持10分以下交易
            tenpay.setParameter("spbill_create_ip", ip);
            tenpay.setParameter("return_url", return_url);
            tenpay.setParameter("partner", bargainor_id);
            tenpay.setParameter("out_trade_no", order.OrderId);
            tenpay.setParameter("notify_url", notify_url);
            tenpay.setParameter("attach", order.UserId.ToString());
            tenpay.setParameter("body", order.Desc);
            tenpay.setParameter("bank_type", order.BankType);


            //系统可选参数
            tenpay.setParameter("sign_type", "MD5");
            tenpay.setParameter("service_version", "1.0");
            tenpay.setParameter("input_charset", ChargeConfig.Charset);
            tenpay.setParameter("sign_key_index", "1");



            string url= tenpay.getRequestURL();

            HttpContext.Current.Response.Redirect(url);

        }
        protected override string OnNotify(System.Web.HttpContext context)
        {
            var handler = new ResponseHandler(context);
            handler.setKey(key);
            if (handler.isTenpaySign())
            {
                ///通知id
                string notify_id = handler.getParameter("notify_id");
                //通过通知ID查询，确保通知来至财富通
                //创建查询请求
                RequestHandler queryReq = new RequestHandler(context);
                queryReq.init();
                queryReq.setKey(key);
                queryReq.setGateUrl("https://gw.tenpay.com/gateway/verifynotifyid.xml");
                queryReq.setParameter("partner", bargainor_id);
                queryReq.setParameter("notify_id", notify_id);
                //通信对象
                TenpayHttpClient httpClient = new TenpayHttpClient();
                httpClient.setTimeOut(5);
                //设置请求内容
                httpClient.setReqContent(queryReq.getRequestURL());
                if (httpClient.call())
                {
                    //设置结果参数
                    ClientResponseHandler queryRes = new ClientResponseHandler();
                    queryRes.setContent(httpClient.getResContent());
                    queryRes.setKey(key);

                    //判断签名及结果
                    //只有签名正确,retcode为0，trade_state为0才是支付成功
                    if (queryRes.isTenpaySign() && queryRes.getParameter("retcode") == "0" && queryRes.getParameter("trade_state") == "0" && queryRes.getParameter("trade_mode") == "1")
                    {
                        string out_trade_no = queryRes.getParameter("out_trade_no");
                        //财富通订单号
                        string transaction_id = queryRes.getParameter("transaction_id");
                        //金额,以分为单位
                        string total_fee = queryRes.getParameter("total_fee");

                        PayHistory order = OnlinePayBusiness.Instance.GetOrder(out_trade_no, ThisCompanyType);
                        if (order == null)
                        {
                            CoreHelper.EventLog.Log(string.Format("在线支付确认时找不到订单{0} 订单号{1}", ThisCompanyType, out_trade_no), true);
                            //context.Response.Write("fail");
                            return "fail";
                        }
                        order.SpBillno = transaction_id;
                        decimal fee = Convert.ToDecimal(total_fee);

                        lock (thisLock)
                        {
                            Confirm(order, GetType(), fee / 100);
                        }
                        //context.Response.Write("success");
                        return "success";
                    }
                }
            }
            //context.Response.Write("fail");
            return "fail";
        }

        public override bool CheckOrder(PayHistory order,out string message)
        {

            //创建请求对象
            RequestHandler reqHandler = new RequestHandler(HttpContext.Current);

            //通信对象
            TenpayHttpClient httpClient = new TenpayHttpClient();

            //应答对象
            ClientResponseHandler resHandler = new ClientResponseHandler();

            //-----------------------------
            //设置请求参数
            //-----------------------------
            reqHandler.init();
            reqHandler.setKey(key);
            reqHandler.setGateUrl("https://gw.tenpay.com/gateway/normalorderquery.xml");

            reqHandler.setParameter("partner", bargainor_id);
            //out_trade_no和transaction_id至少一个必填，同时存在时transaction_id优先
            reqHandler.setParameter("out_trade_no", order.OrderId);
            //reqHandler.setParameter("transaction_id", "1900000109201103020030626316");

            string requestUrl = reqHandler.getRequestURL();
            //设置请求内容
            httpClient.setReqContent(requestUrl);
            //设置超时
            httpClient.setTimeOut(10);

            string rescontent = "";
            //后台调用
            if (httpClient.call())
            {
                //获取结果
                rescontent = httpClient.getResContent();

                resHandler.setKey(key);
                //设置结果参数
                resHandler.setContent(rescontent);

                //判断签名及结果
                if (resHandler.isTenpaySign() && resHandler.getParameter("retcode") == "0")
                {
                    //商户订单号
                    string out_trade_no = resHandler.getParameter("out_trade_no");
                    //财富通订单号
                    string transaction_id = resHandler.getParameter("transaction_id");
                    //金额,以分为单位
                    string total_fee = resHandler.getParameter("total_fee");
                    //如果有使用折扣券，discount有值，total_fee+discount=原请求的total_fee
                    string discount = resHandler.getParameter("discount");
                    //支付结果
                    string trade_state = resHandler.getParameter("trade_state");

                    //支付成功
                    if (trade_state == "0")
                    {
                        if (order.Status != OrderStatus.已确认)
                        {
                            Confirm(order, GetType(), order.Amount);
                        }
                        message = "成功";
                        return true;
                    }
                }
                else
                {
                    message = "失败";
                    return false;
                }
            }
            else
            {
                message = "服务器返回:" + httpClient.getResponseCode();
                return false;
            }
            message = "失败";
            return false;
        }
        
        #endregion

        /// <summary>
        /// 退款接口,可重复提交得到状态
        /// </summary>
        /// <param name="order"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool RefundOrder(PayHistory order, out string message)
        {
            #region 退款
            RequestHandler reqHandler = new RequestHandler(HttpContext.Current);

            //通信对象
            TenpayHttpClient httpClient = new TenpayHttpClient();
            ClientResponseHandler resHandler = new ClientResponseHandler();
            //-----------------------------
            //设置请求参数
            //-----------------------------
            reqHandler.init();
            reqHandler.setKey(key);
            reqHandler.setGateUrl("https://mch.tenpay.com/refundapi/gateway/refund.xml");

            reqHandler.setParameter("partner", bargainor_id);
            //out_trade_no和transaction_id至少一个必填，同时存在时transaction_id优先
            //reqHandler.setParameter("out_trade_no", order.OrderId);
            reqHandler.setParameter("transaction_id", order.OrderId);
            reqHandler.setParameter("out_refund_no", order.OrderId + "C");
            reqHandler.setParameter("total_fee", (order.Amount * 100).ToString());
            reqHandler.setParameter("refund_fee", (order.Amount * 100).ToString());
            reqHandler.setParameter("op_user_id", bargainor_id);
            reqHandler.setParameter("op_user_passwd", ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.PayPass));
            string requestUrl = reqHandler.getRequestURL();
            //证书和证书密码
            httpClient.setCertInfo(ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.CertFile), bargainor_id);
            //设置请求内容
            httpClient.setReqContent(requestUrl);
            //设置超时
            httpClient.setTimeOut(10);

            string rescontent = "";
            //后台调用
            if (!httpClient.call())
            {
                message = "接口调用失败";
                return false;
            }
            //获取结果
            rescontent = httpClient.getResContent();

            resHandler.setKey(key);
            //设置结果参数
            resHandler.setContent(rescontent);
            message = resHandler.getParameter("retmsg");
            //Log("退款返回结果为:" + rescontent);
            //判断签名及结果
            if (!resHandler.isTenpaySign())
            {
                message = "返回签名验证错误";
                return false;
            }
            if (resHandler.getParameter("retcode") != "0")
            {
                return false;
            }

            //商户订单号
            string out_trade_no = resHandler.getParameter("out_trade_no");
            //财富通订单号
            string transaction_id = resHandler.getParameter("transaction_id");

            // 退款状态：
            //4，10：退款成功。
            //3，5，6：退款失败。
            //8，9，11：退款处理中。
            //1，2：未确定，需要商户原退款单号重新发起。
            //7：转入代发，退款到银行发现用户的卡作废或者冻结了，导致原路退款银行卡失败，资金回流到商户的现金帐号，需要商户人工干预，通过线下或者财富通转账的方式进行退款

            string refund_status = resHandler.getParameter("refund_status");

            if (refund_status == "4" || refund_status == "10")
            {
                string recv_user_id = resHandler.getParameter("recv_user_id");
                string reccv_user_name = resHandler.getParameter("reccv_user_name");
                //Log("退款接收人信息为:" + recv_user_id + "  " + reccv_user_name);
                BaseRefundOrder(order);
                return true;
            }
            if (refund_status == "8" || refund_status == "9" || refund_status == "11")
            {
                //todo 自动重复查询
                message = "退款处理中";
                return true;
            }
            return false;

            #endregion
        }
    }
}
