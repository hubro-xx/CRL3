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
    public class MemberManage : CRL.BaseProvider<Member>
    {

        /// <summary>
        /// 实例访问入口
        /// </summary>
        public static MemberManage Instance
        {
            get { return new MemberManage(); }
        }
    }
}
