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

namespace Shopping.Model.Order
{
    public enum OrderStatus
    {
        待付款 = 0,
        下单成功 = 1,
        配货中 = 2,
        配货完成 = 3,
        已发货 = 4,
        确认收货 = 5,
        退货完成 = 6,
        已取消 = 7
    }
}
