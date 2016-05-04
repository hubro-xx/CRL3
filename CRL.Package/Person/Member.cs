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

namespace CRL.Package.Person
{
    /// <summary>
    /// 会员
    /// </summary>
    public class Member : Person
    {
        [Attribute.Field(Length=100)]
        public string PayPass
        {
            get;
            set;
        }
        public string QQ
        {
            get;
            set;
        }
        public string Sex
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
        /// <summary>
        /// 省
        /// </summary>
        public string Province
        {
            get;
            set;
        }
        /// <summary>
        /// 市
        /// </summary>
        public string City
        {
            get;
            set;
        }
        /// <summary>
        /// 区/县
        /// </summary>
        public string County
        {
            get;
            set;
        }
    }
}
