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

namespace CRL.Package.RoleAuthorize
{
    /// <summary>
    /// 用户
    /// </summary>
    [Attribute.Table(TableName = "Employee")]
    public class Employee : Person.Person
    {
        protected override System.Collections.IList GetInitData()
        {
            var list = new List<Employee>();
            //123456
            list.Add(new Employee() { Name = "test", AccountNo = "test", PassWord = "E10ADC3949BA59ABBE56E057F20F883E", Role = 1 });
            return list;
        }
        /// <summary>
        /// qq
        /// </summary>
        public string QQ
        {
            get;
            set;
        }
        /// <summary>
        /// 角色
        /// </summary>
        public int Role
        {
            get;
            set;
        }
        /// <summary>
        /// 角色名称
        /// </summary>
        public String RoleName
        {
            get{
                return RoleBusiness.Instance.QueryItem(b => b.Id == Role).Name;
            } 
        }
        [CRL.Attribute.Field(Length=100)]
        public string Token
        {
            get;
            set;
        }
        public string Ip
        {
            get;
            set;
        }
        public DateTime? Birthday
        {
            get;
            set;
        }
        public string Sex
        {
            get;
            set;
        }
        /// <summary>
        /// 身份证号
        /// </summary>
        [CRL.Attribute.Field(Length = 50)]
        public string IdentityNo
        {
            get;
            set;
        }
        [Attribute.Field(Length = 100)]
        public string Address
        {
            get;
            set;
        }
        [Attribute.Field(Length = 100)]
        public string HeadImg
        {
            get;
            set;
        }
        /// <summary>
        /// 部门
        /// </summary>
        public string Department
        {
            get;
            set;
        }
    }
}
