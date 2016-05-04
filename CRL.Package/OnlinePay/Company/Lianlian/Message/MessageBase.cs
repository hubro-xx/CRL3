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

namespace CRL.Package.OnlinePay.Company.Lianlian.Message
{
    public class MessageBase
    {
        public virtual string InterFaceUrl
        {
            get
            {
                return "";
            }
        }
        /// <summary>
        /// 商户编号
        /// </summary>
        public string oid_partner;
        /// <summary>
        /// 签名方式
        /// </summary>
        public string sign_type;
        /// <summary>
        /// 签名
        /// </summary>
        public string sign;

        SortedDictionary<string, string> GetDic()
        {
            var fields = GetType().GetFields();
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            foreach (var item in fields)
            {
                if (item.Name != "sign")
                {
                    dic.Add(item.Name, item.GetValue(this) + "");
                }
            }
            return dic;
        }
        public void SetSign()
        {
            var dic = GetDic();
            string sign2 = YinTongUtil.addSign(dic, PartnerConfig.TRADER_PRI_KEY, PartnerConfig.MD5_KEY);
            sign = sign2;
        }
        public bool CheckSign()
        {
            var dic = GetDic();
            var a = YinTongUtil.checkSign(dic, PartnerConfig.YT_PUB_KEY, //验证失败
                 PartnerConfig.MD5_KEY);
            return a;
        }
        public static T FromRequest<T>(string json) where T : class,new()
        {
            var obj = CoreHelper.SerializeHelper.SerializerFromJSON(json, typeof(T), Encoding.UTF8);
            return obj as T;
        }
        public static T FromRequest<T>(System.Collections.Specialized.NameValueCollection c) where T : class,new()
        {
            var fields = typeof(T).GetFields();
            var obj = new T();
            foreach (var item in fields)
            {
                item.SetValue(obj, c[item.Name]);
            }
            return obj;
        }
    }
}
