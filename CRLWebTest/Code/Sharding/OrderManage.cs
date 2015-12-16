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