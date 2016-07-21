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
    /// <summary>
    /// 订单
    /// </summary>
    [CRL.Attribute.Table(TableName = "OrderProduct")]//重新指定对应的表名
    public class Order : CRL.IModelBase
    {
        protected override System.Collections.IList GetInitData()
        {
            var list = new List<Order>();
            list.Add(new Order() { UserId = 1, OrderId = "123" });
            list.Add(new Order() { UserId = 2, OrderId = "456" });
            return list;
        }
        public int Status
        {
            get;
            set;
        }
        public string OrderId
        {
            get;
            set;
        }
        public string Remark
        {
            get;
            set;
        }
        public int UserId
        {
            get;
            set;
        }
        //新增Channel属性
        public string Channel
        {
            get;
            set;
        }
        
    }
}
