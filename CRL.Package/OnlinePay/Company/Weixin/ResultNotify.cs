/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CRL.Package.OnlinePay.Company.Weixin
{
    /// <summary>
    /// 支付结果通知回调处理类
    /// 负责接收微信支付后台发送的支付结果并对订单有效性进行验证，将验证结果反馈给微信支付后台
    /// </summary>
    public class ResultNotify:Notify
    {
        public ResultNotify(System.Web.HttpContext page)
            : base(page)
        {
        }

        public override bool ProcessNotify(out string error, out string out_trade_no)
        {
            WxPayData notifyData = GetNotifyData();
            out_trade_no = "";
            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                error = "支付结果中微信订单号不存在";
                return false;
            }
            out_trade_no = notifyData.GetValue("out_trade_no").ToString() ;
            string transaction_id = notifyData.GetValue("transaction_id").ToString();

            //查询订单，判断订单真实性
            if (!QueryOrder(transaction_id))
            {
                error = "订单查询失败";
                return false;
            }
            //查询订单成功
            else
            {
                //WxPayData res = new WxPayData();
                //res.SetValue("return_code", "SUCCESS");
                //res.SetValue("return_msg", "OK");
                //Log.Info(this.GetType().ToString(), "order query success : " + res.ToXml());
                //page.Response.Write(res.ToXml());
                //page.Response.End();
                error = "";
                return true;
            }
        }

        //查询订单
        private bool QueryOrder(string transaction_id)
        {
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transaction_id);
            WxPayData res = WxPayApi.OrderQuery(req);
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
                res.GetValue("result_code").ToString() == "SUCCESS")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
