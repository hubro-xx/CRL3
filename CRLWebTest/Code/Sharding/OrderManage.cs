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

namespace WebTest.Code.Sharding
{
    public class OrderSharding : CRL.IModelBase
    {
        public int MemberId
        {
            get;
            set;
        }
        public string Remark
        {
            get;
            set;
        }
    }
    public class OrderManage : CRL.Sharding.BaseProvider<OrderSharding>
    {
        public static OrderManage Instance
        {
            get { return new OrderManage(); }
        }
    }
}
