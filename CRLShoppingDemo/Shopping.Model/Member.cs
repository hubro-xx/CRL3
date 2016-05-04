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
using System.Threading.Tasks;

namespace Shopping.Model
{
    /// <summary>
    /// 会员
    /// </summary>
    public class Member : CRL.Package.Person.Member
    {
        /// <summary>
        /// 初始数据
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.IList GetInitData()
        {
            var list = new List<Member>();
            //123456
            list.Add(new Member() { Name = "Member1", AccountNo = "Member1", PassWord = "E10ADC3949BA59ABBE56E057F20F883E" });
            return list;
        }
        public override string CheckData()
        {
            return base.CheckData();
        }
       
        /// <summary>
        /// 推荐人
        /// </summary>
        public int Recommended
        {
            get;
            set;
        }
        /// <summary>
        /// 头像
        /// </summary>
        [CRL.Attribute.Field(Length = 100)]
        public string HeadImg
        {
            get;
            set;
        }
        
    }
}
