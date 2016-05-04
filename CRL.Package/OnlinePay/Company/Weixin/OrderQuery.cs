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

namespace CRL.Package.OnlinePay.Company.Weixin
{
    public class OrderQuery
    {
        /***
        * 订单查询完整业务流程逻辑
        * @param transaction_id 微信订单号（优先使用）
        * @param out_trade_no 商户订单号
        * @return 订单查询结果（xml格式）
        */
        public static bool Run(string transaction_id, string out_trade_no,out string error)
        {
            //Log.Info("OrderQuery", "OrderQuery is processing...");

            WxPayData data = new WxPayData();
            if(!string.IsNullOrEmpty(transaction_id))//如果微信订单号存在，则以微信订单号为准
            {
                data.SetValue("transaction_id", transaction_id);
            }
            else//微信订单号不存在，才根据商户订单号去查单
            {
                data.SetValue("out_trade_no", out_trade_no);
            }

            WxPayData result = WxPayApi.OrderQuery(data);//提交订单查询请求给API，接收返回数据
            var a = result.GetValue("return_code").ToString() == "SUCCESS";
            error = result.GetValue("return_msg").ToString();
            return a;
        }
    }
}
