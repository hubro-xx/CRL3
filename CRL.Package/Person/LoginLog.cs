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
    public class LoginLog : CRL.IModelBase
    {
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int UserId
        {
            get;
            set;
        }
        public string UserName
        {
            get;
            set;
        }
        /// <summary>
        /// 来源
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string Source
        {
            get;
            set;
        }
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string UserType
        {
            get;
            set;
        }
        public string RemoteIp
        {
            get;
            set;
        }
        [Attribute.Field(Length=200)]
        public string Browser
        {
            get;
            set;
        }
    }
}
