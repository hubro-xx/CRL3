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
using System.Web;
namespace CRL.Package.Person
{
    /// <summary>
    /// 读写短信验证数据
    /// </summary>
    public class MobileVerifyData
    {
        /// <summary>
        /// 写入手机信息
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="code"></param>
        /// <param name="receiveMobile"></param>
        public static void SetCode(string moduleName, string code, string receiveMobile)
        {
            var item = new SmsSendRecord() { Mobile = receiveMobile, Code = code, ModuleName = moduleName };
            SmsSendRecordManage.Instance.Add(item);
        }
        /// <summary>
        /// 通过模块名称获取验证码
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static string GetCode(string moduleName, string mobile)
        {
            var item = SmsSendRecordManage.Instance.QueryItem(b => b.ModuleName == moduleName && b.Mobile == mobile, true);
            if (item == null)
                return null;
            if (item.Expired)
            {
                return null;
            }
            return item.Code;
        }
        /// <summary>
        /// 获取验证码读取次数,用来作次数限制
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static int GetTimes(string moduleName, string mobile)
        {
            return 0;
        }
        /// <summary>
        /// 通过模块名称获取发送间隔
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static int GetSendTimeDiff(string moduleName, string mobile)
        {
            var item = SmsSendRecordManage.Instance.QueryItem(b => b.ModuleName == moduleName && b.Mobile == mobile, true);
            if (item == null)
            {
                return 99999;
            }
            return (int)(DateTime.Now - item.AddTime).TotalSeconds;
        }
        /// <summary>
        /// 获取本次验证码发送次数
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static int GetTotalSendTimes(string moduleName, string mobile)
        {
            var date = DateTime.Now.AddMinutes(-30);
            return SmsSendRecordManage.Instance.Count(b => b.ModuleName == moduleName && b.Mobile == mobile && b.AddTime > date);
        }
        /// <summary>
        /// 清除本次数据
        /// </summary>
        /// <param name="moduleName"></param>
        public static void Clear(string moduleName, string mobile)
        {
            SmsSendRecordManage.Instance.Delete(b => b.ModuleName == moduleName && b.Mobile == mobile);
        }
    }
}
