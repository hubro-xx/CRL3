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

namespace CRL.Package
{
    /// <summary>
    /// 业务封装配置
    /// </summary>
    public class SettingConfig
    {
        /// <summary>
        /// 订单退款时
        /// 需判断订单类型
        /// </summary>
        public static Action<Package.OnlinePay.PayHistory> OnlinePayOrderRefund;
        /// <summary>
        /// 订单确认成功时
        /// 需判断订单类型
        /// </summary>
        public static Action<Package.OnlinePay.PayHistory> OnlinePayConfirmOrder;

        /// <summary>
        /// 权限控制密码加密
        /// </summary>
        public static Func<string, string> RoleAuthorizeEncryptPass;

    }
}
