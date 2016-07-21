/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTest.Code
{
    public class ClassGuid:CRL.IModel
    {
        [CRL.Attribute.Field(IsPrimaryKey=true,KeepIdentity=true)]
        public Guid id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
    }
    public class ClassGuidManage : CRL.BaseProvider<ClassGuid>
    {
        public static ClassGuidManage Instance
        {
            get { return new ClassGuidManage(); }
        }
    }
}
