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
using System.Collections;

namespace CRL.Package.Person
{
    public class PersonBusiness<TType, TModel> : PersonBusiness<TModel>
        where TModel : Person, new()
    {
    }
    /// <summary>
    /// 会员/人维护
    /// </summary>
    public class PersonBusiness<TModel> : BaseProvider<TModel>
        where TModel : Person, new()
    {
        /// <summary>
        /// 加密方法,默认MD5,如不同请重写
        /// </summary>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public virtual string EncryptPass(string passWord)
        {
            if (SettingConfig.RoleAuthorizeEncryptPass != null)
            {
                return SettingConfig.RoleAuthorizeEncryptPass(passWord);
            }
            return CoreHelper.StringHelper.EncryptMD5(passWord); 
        }
        /// <summary>
        /// 检测帐号是否存在
        /// </summary>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        public bool CheckAccountExists(string accountNo)
        {
            var item = QueryItem(b => b.AccountNo == accountNo);
            return item != null;
        }
        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="passWord"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool CheckPass(string accountNo, string passWord, out string message)
        {
            message = "";
            accountNo = accountNo.Trim();
            var item = QueryItem(b => b.AccountNo == accountNo);
            if (item == null)
            {
                message = "帐号不存在";
                return false;
            }
            item.PassWord = item.PassWord + "";
            bool a = item.PassWord.ToUpper() == EncryptPass(passWord).ToUpper();
            if (!a)
            {
                message = "密码不正确";
            }
            if (item.Locked)
            {
                message = "帐号已被锁定";
                return false;
            }
            return a;
        }
        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="passWord"></param>
        public void UpdatePass(string accountNo, string passWord)
        {
            ParameCollection c2 = new ParameCollection();
            c2["PassWord"] = EncryptPass(passWord);
            int n = Update(b => b.AccountNo == accountNo, c2);
            if (n == 0)
            {
                throw new Exception("修改密码失败,账号不正确");
            }
        }
        /// <summary>
        /// 修改付款密码
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="passWord"></param>
        public void UpdatePayPass(string accountNo, string passWord)
        {
            ParameCollection c2 = new ParameCollection();
            c2["PayPass"] = EncryptPass(passWord);
            int n = Update(b => b.AccountNo == accountNo, c2);
            if (n == 0)
            {
                throw new Exception("修改密码失败,账号不正确");
            }
        }
        #region 登录验证
        /// <summary>
        /// 使用当前用户写入登录票据
        /// 写入值为 Id,Name,RuleName,TagData
        /// </summary>
        /// <param name="user"></param>
        /// <param name="rules">用户组</param>
        /// <param name="expires">是否自动过期</param>
        public void Login(TModel user, string rules, bool expires)
        {
            user.RuleName = rules;
            //要设置域请在WEB.CONFIG设置
            System.Web.Security.FormsAuthentication.SignOut();
            CoreHelper.FormAuthentication.AuthenticationSecurity.SetTicket(user, rules, expires);
        }
        /// <summary>
        /// 登出
        /// </summary>
        public void LoginOut()
        {
            CoreHelper.FormAuthentication.AuthenticationSecurity.LoginOut();
        }
        /// <summary>
        /// 检查登录票据
        /// Application_AuthenticateRequest使用
        /// </summary>
        public void CheckTicket()
        {
            CoreHelper.FormAuthentication.AuthenticationSecurity.CheckTicket();
        }
        /// <summary>
        /// 获取当前登录人对象
        /// </summary>
        /// <returns></returns>
        public TModel GetCurrent()
        {
            return QueryItem(b => b.Id == CurrentUser.Id);
        }
        /// <summary>
        /// 获取当前登录的用户
        /// 请用ID取详细信息
        /// </summary>
        public static Person CurrentUser
        {
            get
            {
                string userTicket = System.Web.HttpContext.Current.User.Identity.Name;
                if (string.IsNullOrEmpty(userTicket))
                    return null;
                //数据不对会造成空
                var user = Person.ConverFromArry(userTicket);
                if (user == null)
                {
                    return null;
                }
                return user;
            }
        }
        #endregion

        /// <summary>
        /// 记录登录日志
        /// </summary>
        /// <param name="log"></param>
        public virtual void SaveLoginLog(LoginLog log)
        {
            var helper = DBExtend;
            try
            {
                helper.InsertFromObj(log);
            }
            catch { }
        }
    }
}
