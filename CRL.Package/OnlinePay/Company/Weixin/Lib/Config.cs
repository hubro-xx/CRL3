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
    /**
    * 	配置账号信息
    */
    public class WxPayConfig
    {
        //=======【基本信息设置】=====================================
        /* 微信公众号信息配置
        * APPID：绑定支付的APPID（必须配置）
        * MCHID：商户号（必须配置）
        * KEY：商户支付密钥，参考开户邮件设置（必须配置）
        * APPSECRET：公众帐号secert（仅JSAPI支付的时候需要配置）
        */
        public static string APPID = CoreHelper.CustomSetting.GetConfigKey("Weixin_APPID");
        public static string MCHID = ChargeConfig.GetConfigKey(CompanyType.微信, ChargeConfig.DataType.User);
        public static string KEY = ChargeConfig.GetConfigKey(CompanyType.微信, ChargeConfig.DataType.Key);
        public static string APPSECRET = CoreHelper.CustomSetting.GetConfigKey("Weixin_APPSECRET");

        //=======【证书路径设置】===================================== 
        /* 证书路径,注意应该填写绝对路径（仅退款、撤销订单时需要）
        */
        public static string SSLCERT_PATH = ChargeConfig.GetConfigKey(CompanyType.微信, ChargeConfig.DataType.CertFile);
        public static string SSLCERT_PASSWORD = ChargeConfig.GetConfigKey(CompanyType.微信, ChargeConfig.DataType.CertFilePass);

        public static string CurrentHost
        {
            get
            {
                string url = HttpContext.Current.Request.Url.ToString();
                string[] arry = url.Split('/');
                string host = arry[2];
                string url1 = arry[0] + "//" + host;
                //todo 更改主机URL
                
                return url1;
            }
        }
        public static string GetAbsUrl(string url)
        {
            if (url.StartsWith("http"))
            {
                return url;
            }
            return CurrentHost + url;
        }

        //=======【支付结果通知url】===================================== 
        /* 支付结果通知回调url，用于商户接收支付结果
        */
        public static string NOTIFY_URL = GetAbsUrl(ChargeConfig.GetConfigKey(CompanyType.微信, ChargeConfig.DataType.NotifyUrl));

        //=======【商户系统后台机器IP】===================================== 
        /* 此参数可手动配置也可在程序中自动获取
        */
        public static string IP = CoreHelper.RequestHelper.GetServerIp();


        //=======【代理服务器设置】===================================
        /* 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
        */
        public const string PROXY_URL = "";

        //=======【上报信息配置】===================================
        /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        */
        public const int REPORT_LEVENL = 1;

        //=======【日志级别】===================================
        /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
        */
        public const int LOG_LEVENL = 0;
    }
}
