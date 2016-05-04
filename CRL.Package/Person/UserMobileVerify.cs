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
using CoreHelper;
namespace CRL.Package.Person
{
    /// <summary>
    /// 对用户进行手机验证
    /// 验证信息存留在服务端
    /// </summary>
    public class UserMobileVerify
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        protected virtual string ModuleName
        {
            get
            {
                return "MobileVerify";
            }
        }
        /// <summary>
        /// 重复发送间隔
        /// </summary>
        protected virtual int NextSendSencond
        {
            get
            {
                return 120;
            }
        }
        /// <summary>
        /// 发送短信方法,请重写
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="content"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected virtual bool SendSms(string mobile, string content, out string error)
        {
            error = "请重写此方法";
            return false;
        }
        /// <summary>
        /// 当前随机码
        /// </summary>
        public string CurrentCode;
        /// <summary>
        /// 生成随机码
        /// </summary>
        /// <returns></returns>
        string MakeRandomCode()
        {
            string chkCode;
            string character = "0123456789";
            //character += "ABCDEFGHIJKLMNPQRSTUVWXYZ";
            chkCode = "";
            Random rnd = new Random();
            //生成验证码字符串
            for (int i = 0; i < 6; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }
            return chkCode;
        }
        /// <summary>
        /// 发送自定义格式短信
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="content">不为空时为自定义短信,验证码域请用{0}表示</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool SendCode(string mobile, string content, out string error)
        {
            error = "";
            if (string.IsNullOrEmpty(mobile))
            {
                error = "发送失败,没有传入手机号";
                return false;
            }
            if (!mobile.IsCellPhone())
            {
                error = "手机号格式不正确";
                return false;
            }
            string chkCode = MakeRandomCode();

            if (string.IsNullOrEmpty(content))
            {
                content = "您本次操作的验证码为 " + chkCode + ",切勿将此验证码泄露给他人,如非本人操作请忽略";
            }
            else
            {
                content = string.Format(content, chkCode);
            }
            double n = MobileVerifyData.GetSendTimeDiff(ModuleName, mobile);
            if (n < NextSendSencond)
            {
                error = "发送失败,验证码距上次发送不足" + NextSendSencond + "秒,请稍后再试";
                return false;
            }
            int totalSend = MobileVerifyData.GetTotalSendTimes(ModuleName, mobile);
            if (totalSend > 5)
            {
                error = "发送失败,请求超过了限制,请稍后再试";
                return false;
            }
            //bool a = CoreHelper.SMSMessage.Send(mobile, content, out error); //老短信接口
            CurrentCode = chkCode;
            bool a = SendSms(mobile, content, out error);
            if (a)
            {
                MobileVerifyData.SetCode(ModuleName, chkCode, mobile.Trim());
                string str = string.Format("时间:{0} 验证码:{1} 模块:{2} \r\n", DateTime.Now.ToString("yy-MM-dd HH:mm:ss fffff"), chkCode, ModuleName);
            }
            return a;
        }

        /// <summary>
        /// 检验验证码,成功会清除缓存数据
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool CheckCode(string mobile, string code, out string error)
        {
            error = "";
            if (MobileVerifyData.GetTimes(ModuleName, mobile) > 10)
            {
                error = "调用次数超过了限制";
                return false;
            }
            string code1 = MobileVerifyData.GetCode(ModuleName, mobile);
            if (string.IsNullOrEmpty(code1))
            {
                error = "找不到验证码或已过期,请重新发送";
                return false;
            }
            code = code.ToLower();
            code1 = code1.ToLower();
            bool a = code == code1;
            if (a)
            {
                MobileVerifyData.Clear(ModuleName, mobile);
            }
            else
            {
                error = "验证码不正确";
            }
            return a;
        }
    }
}
