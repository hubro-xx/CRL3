/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Xml;

namespace CRL.Package.OnlinePay.Company.AlipayWap
{
    /// <summary>
    /// 类名：Service
    /// 功能：支付宝各接口构造类
    /// 详细：构造支付宝各接口请求参数
    /// 版本：3.0
    /// 日期：2012-07-11
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考
    /// </summary>
    public class Service
    {
        /// <summary>
        /// 构造wap交易创建接口
        /// </summary>
        /// <param name="requrl">请求地址</param>
        /// <param name="subject">商品名称</param>
        /// <param name="outTradeNo">外部交易号</param>
        /// <param name="totalFee">商品总价</param>
        /// <param name="sellerAccountName">卖家账户</param>
        /// <param name="notifyUrl">商户接收通知URL</param>
        /// <param name="outUser">商户用户唯一ID</param>
        /// <param name="merchantUrl">返回商户URL</param>
        /// <param name="callbackurl">支付成功跳转链接</param>
        /// <param name="service">服务</param>
        /// <param name="secid">签名方式</param>
        /// <param name="partner">合作伙伴ID</param>
        /// <param name="reqid">商户请求ID</param>
        /// <param name="format">请求参数格式</param>
        /// <param name="version"></param>
        /// <param name="input_charset">编码格式</param>
        /// <param name="gatway">网关</param>
        /// <param name="key">MD5校验码</param>
        /// <param name="sign_type">签名类型</param>
        /// <returns>返回token</returns>
        public string alipay_wap_trade_create_direct(
            string requrl, 
            string subject, 
            string outTradeNo, 
            string totalFee, 
            string sellerAccountName, 
            string notifyUrl, 
            string outUser, 
            string merchantUrl, 
            string callbackurl, 
            string service,
            string secid, 
            string partner, 
            string reqid, 
            string format, 
            string version,
            string input_charset,
            string gatway,
            string key,
            string sign_type)
        {
            //临时请求参数数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
             
            //构造请求参数数组
            string req_Data = "<direct_trade_create_req><subject>" + subject + "</subject><out_trade_no>" +
                outTradeNo + "</out_trade_no><total_fee>" + totalFee + "</total_fee><seller_account_name>" + sellerAccountName +
                "</seller_account_name><notify_url>" + notifyUrl + "</notify_url><out_user>" + outUser +
                "</out_user><merchant_url>" + merchantUrl + "</merchant_url>" +
                "<call_back_url>" + callbackurl + "</call_back_url></direct_trade_create_req>";

            sParaTemp.Add("req_data" , req_Data);
            sParaTemp.Add("service" , service);
            sParaTemp.Add("sec_id" , secid);
            sParaTemp.Add("partner" , partner);
            sParaTemp.Add("req_id" , reqid);
            sParaTemp.Add("format" , format);
            sParaTemp.Add("v" , version);

            //构造表单提交HTML数据
            string strResult = Submit.SendPostInfo(sParaTemp, gatway, input_charset, key, sign_type);


            //对返回字符串处理，得到request_token的值
            strResult = HttpUtility.UrlDecode(strResult, Encoding.GetEncoding(input_charset));
            //分解返回数据 用&拆分赋值给result
            string[] result = strResult.Split('&');

            string res_data = string.Empty;
            
            //-------------------------------此处代码有bug，已注释---------------------------
            ////AlipayService.cs 124行代码修改
            //if (result.Length > 0)
            //    //替换成标准Xml数据
            //    res_data = result[0].Replace("res_data=", string.Empty);
            //---------------------------------------------------------------------------------------

            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].IndexOf("res_data=") >= 0)
                {
                    res_data = result[i].Replace("res_data=", string.Empty);
                }
            }
            
            //得到 request_token 的值
            string token = string.Empty;
            try
            {
                token = Function.GetStrForXmlDoc(res_data, "direct_trade_create_res/request_token");
            }
            catch
            {
                //提示 返回token值无效
                return string.Empty;
            }
            return token;

        }

        /// <summary>
        /// 授权并执行
        /// </summary>
        /// <param name="requrl">请求地址</param>
        /// <param name="secid">签名方式</param>
        /// <param name="partner">合作伙伴ID</param>
        /// <param name="callbackurl">支付成功跳转链接</param>
        /// <param name="format">请求参数格式</param>
        /// <param name="version">版本</param>
        /// <param name="service">服务</param>
        /// <param name="token">返回token</param> 
        /// <param name="input_charset">编码格式</param>
        /// <param name="gatway">网关</param>
        /// <param name="key">MD5校验码</param>
        /// <param name="sign_type">签名类型</param>
        /// <returns>直接跳转</returns>
        public string alipay_Wap_Auth_AuthAndExecute(
            string requrl,
            string secid,
            string partner,
            string callbackurl,
            string format,
            string version,
            string service,
            string token,
            string input_charset,
            string gatway,
            string key,
            string sign_type)
        {
            //临时请求参数数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            //拼接req_data
            string req_Data = "<auth_and_execute_req><request_token>" + token + "</request_token></auth_and_execute_req>";

            sParaTemp.Add("req_data" , req_Data);
            sParaTemp.Add("service" , service);
            sParaTemp.Add("sec_id" , secid);
            sParaTemp.Add("partner" , partner);
            sParaTemp.Add("format" , format);
            sParaTemp.Add("v" , version);
            
            //返回拼接后的跳转URL
            return Submit.SendPostRedirect(sParaTemp, gatway, input_charset, key, sign_type);
        }
    }
}
