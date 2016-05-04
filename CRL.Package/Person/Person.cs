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
using System.ComponentModel.DataAnnotations;
namespace CRL.Package.Person
{
    /// <summary>
    /// 会员/人
    /// </summary>
    public class Person : IModelBase, CoreHelper.FormAuthentication.IUser
    {
        /// <summary>
        /// 用户组仅在验证时用
        /// </summary>
        public string RuleName;
        /// <summary>
        /// 存入自定义数据
        /// </summary>
        public string TagData;
        public override string CheckData()
        {
            return "";
        }
        #region FORM验证存取
        /// <summary>
        /// 转为登录用的IUSER
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Person ConverFromArry(string content)
        {
            string[] arry = content.Split('|');
            Person p = new Person();
            p.Id = Convert.ToInt32(arry[0]);
            p.Name = arry[1];
            p.RuleName = arry[2];
            if (arry.Length > 3)
            {
                p.TagData = arry[3];
            }
            return p;
        }
        /// <summary>
        /// 转为可存储的STRING
        /// </summary>
        /// <returns></returns>
        public string ToArry()
        {
            return string.Format("{0}|{1}|{2}|{3}", Id, Name, RuleName, TagData);
        }
        #endregion
        /// <summary>
        /// 名称
        /// </summary>
        [CRL.Attribute.Field(Length = 50)]
        [Required]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 帐号
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集唯一, Length = 50)]
        [Required]
        public string AccountNo
        {
            get;
            set;
        }
        [Attribute.Field(Length = 100)]
        [Required]
        public string PassWord
        {
            get;
            set;
        }
        [Attribute.Field(MapingField = false)]
        [Required]
        public string VerifyCode
        {
            get;
            set;
        }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get;
            set;
        }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile
        {
            get;
            set;
        }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool Locked
        {
            get;
            set;
        }
        /// <summary>
        /// 注册IP
        /// </summary>
        public string RegisterIp
        {
            get;
            set;
        }
    }
}
