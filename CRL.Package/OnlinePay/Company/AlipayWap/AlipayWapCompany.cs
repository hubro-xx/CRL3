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
namespace CRL.Package.OnlinePay.Company.AlipayWap
{
    public class AlipayWapCompany : CompanyBase
    {
        public override CompanyType ThisCompanyType
        {
            get { return CompanyType.支付宝WAP; }
        }
        public SortedDictionary<string, string> GetRequestGet(System.Web.HttpContext context)
        {
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            //对url第一个字符？过滤
            string query = context.Request.Url.Query.Replace("?", "");
            if (!string.IsNullOrEmpty(query))
            {
                //根据&符号分隔成数组
                string[] coll = query.Split('&');
                //定义临时数组
                string[] temp = { };
                //循环各数组
                for (int i = 0; i < coll.Length; i++)
                {
                    //根据=号拆分
                    temp = coll[i].Split('=');
                    //把参数名和值分别添加至SortedDictionary数组
                    sArray.Add(temp[0], temp[1]);
                }
            }
            return sArray;
        }
        public bool OnReturn(System.Web.HttpContext context,out string error)
        {
            error = "";
            SortedDictionary<string, string> sArrary = GetRequestGet(context);
            var config = new Config(new PayHistory());
            //生成本地签名sign
            string strSign = Function.BuildMysign(sArrary, config.Key, config.Sec_id, config.Input_charset_UTF8);

            //获取支付宝返回sign
            string aliSign = context.Request["sign"];

            //验签对比
            if (!aliSign.Equals(strSign))
            {
                error = "验签失败";
                return false;
            }

            string result = context.Request["result"];
            //比较result值是否为success
            if (!result.Equals("success"))
            {
                //交易未成功
                error = "交易未成功";
                return false;
            }
            else
            {
                var out_trade_no = context.Request["out_trade_no"];
                var order = OnlinePayBusiness.Instance.GetOrder(out_trade_no, ThisCompanyType);
                Confirm(order, GetType(), order.Amount);
                return true;
            }
        }
        protected override string OnNotify(System.Web.HttpContext context)
        {
            Dictionary<string, string> sArrary = new Dictionary<string, string>();
            sArrary.Add("service", context.Request.Form["service"]);
            sArrary.Add("v", context.Request.Form["v"]);
            sArrary.Add("sec_id", context.Request.Form["sec_id"]);
            sArrary.Add("notify_data", context.Request.Form["notify_data"]);
            var config = new Config(new PayHistory());
            //生成签名，用于和post过来的签名进行对照
            string mysign = Function.BuildMysign(sArrary, config.Key, config.Sec_id, config.Input_charset_UTF8);
            //支付宝post的签名
            string aliSign = context.Request.Form["sign"];

            if (!aliSign.Equals(mysign))
            {
                //签名验证失败
                //Response.Write("fail");
                return "fail";
            }

            //获取notify_data的值
            string notify_data = context.Request.Form["notify_data"];
            //获取 notify_data 参数中xml格式里面的 trade_status 值
            string trade_status = Function.GetStrForXmlDoc(notify_data, "notify/trade_status");

            //判断trade_status是否为TRADE_FINISHED
            if (!trade_status.Equals("TRADE_FINISHED"))
            {
                //交易未成功
                return "fail";
            }
            else
            {
                //交易成功并在页面返回success
                string out_trade_no = Function.GetStrForXmlDoc(notify_data, "notify/out_trade_no");
                var order = OnlinePayBusiness.Instance.GetOrder(out_trade_no, ThisCompanyType);
                Confirm(order, GetType(), order.Amount);
                return "success";
                
            }
        }

        public override void Submit(PayHistory order)
        {
            //初始化Service
            Service ali = new Service();
            var context = HttpContext.Current;
            var config = new Config(order);
            config.Notify_url = NotifyUrl;
            config.Call_back_url = ReturnUrl;
            //创建交易接口
            string token = ali.alipay_wap_trade_create_direct(
               config.Req_url, config.Subject, config.Out_trade_no, config.Total_fee, config.Seller_account_name, config.Notify_url,
               config.Out_user, config.Merchant_url, config.Call_back_url, config.Service_Create, config.Sec_id, config.Partner, config.Req_id, config.Format, config.V, config.Input_charset_UTF8, config.Req_url, config.Key, config.Sec_id);

            //构造，重定向URL
            string url = ali.alipay_Wap_Auth_AuthAndExecute(config.Req_url, config.Sec_id, config.Partner, config.Call_back_url, config.Format, config.V, config.Service_Auth, token, config.Input_charset_UTF8, config.Req_url, config.Key, config.Sec_id);
            //跳转收银台支付页面
            System.Web.HttpContext.Current.Response.Redirect(url);
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
