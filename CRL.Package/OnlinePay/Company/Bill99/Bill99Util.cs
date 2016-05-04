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

namespace CRL.Package.OnlinePay.Company.Bill99
{
    public class Bill99Util
    {
        #region 快捷支付
        ///// <summary>
        ///// PCI存储
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public static PciDataContent.Response PCIStore(PciDataContent.Request request)
        //{
        //    var xmlDoc = request.SendRequest();
        //    var response = PciDataContent.Response.FromXml(xmlDoc);
        //    return response;
        //}
        ///// <summary>
        ///// PCI查询
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public static PciQueryContent.Response PCIStore(PciQueryContent.Request request)
        //{
        //    var xmlDoc = request.SendRequest();
        //    var response = PciQueryContent.Response.FromXml(xmlDoc);
        //    return response;
        //}
        /// <summary>
        /// 动态鉴权
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool PCIStore(GetDynNumContent.Request request, bool first, out string token)
        {
            request.externalRefNumber = DateTime.Now.ToString("yyyyMMddHHmmss");//必填
            request.storablePan = request.pan.Substring(0, 6) + request.pan.Substring(request.pan.Length - 4);
            //以下第二次鉴权可以不需要
            if (!first)
            {
                request.cardHolderName = "";
                request.idType = "";
                request.cardHolderId = "";
                request.pan = "";
                request.expiredDate = "";
                request.phoneNO = "";
                request.cvv2 = "";
            }
            var xmlDoc = request.SendRequest();
            var response = GetDynNumContent.Response.FromXml(xmlDoc);
            token = response.token;
            return response.responseCode == "00";
        }
        /// <summary>
        /// 快捷支付
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool Purchase(TxnMsgContent.Request request, bool first, out string error)
        {
            request.externalRefNumber = DateTime.Now.ToString("yyyyMMddHHmmss");//必填
            request.entryTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            request.terminalId = CoreHelper.CustomSetting.GetConfigKey("99billterminalId");
            //以下第二次鉴权可以不需要
            if (!first)
            {
                request.cardNo = "";
                request.expiredDate = "";
                request.cvv2 = "";
                request.cardHolderName = "";
                request.cardHolderId = "";
                request.idType = "";
            }
            request.txnType = "PUR";
            request.interactiveStatus = "TR1";
            request.entryTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string str = "";
            var extDate = request.extData;
            str += "<extMap>";
            str += "<extDate><key>phone</key><value>" + extDate.phone + "</value></extDate>";
            str += "<extDate><key>validCode</key><value>" + extDate.validCode + "</value></extDate>";
            str += "<extDate><key>savePciFlag</key><value>" + extDate.savePciFlag + "</value></extDate>";
            str += "<extDate><key>token</key><value>" + extDate.token + "</value></extDate>";
            str += "<extDate><key>payBatch</key><value>" + extDate.payBatch + "</value></extDate>";
            str += "</extMap>";
            request.OtherMsg = str;
            var xmlDoc = request.SendRequest();
            var response = TxnMsgContent.Response.FromXml(xmlDoc);
            error = response.responseTextMessage;
            return response.responseCode=="00";
        }
        #endregion
    }
}
