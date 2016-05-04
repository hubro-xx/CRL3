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

namespace CRL.Package.OnlinePay.Company.Bill99.GetDynNumContent
{
    //动态鉴权
    public class Request : PCIBase
    {
        public override string InterfacePath
        {
            get
            {
                return "/cnp/getDynNum";
            }
        }
        /// <summary>
        /// 外部跟踪编号
        /// </summary>
        public string externalRefNumber;
        /// <summary>
        /// 银行代码
        /// </summary>
        public string bankId;
        /// <summary>
        /// 金额
        /// </summary>
        public string amount;

        #region 第二次时不需要传
        /// <summary>
        /// 持卡人姓名
        /// </summary>
        public string cardHolderName;
        /// <summary>
        /// 证件类型
        /// </summary>
        public string idType;
        /// <summary>
        /// 持卡人身份证号
        /// </summary>
        public string cardHolderId;
        /// <summary>
        /// 卡号
        /// </summary>
        public string pan;
        /// <summary>
        /// 卡效期
        /// </summary>
        public string expiredDate;
        /// <summary>
        /// 电话
        /// </summary>
        public string phoneNO;
        /// <summary>
        /// 卡验证码
        /// </summary>
        public string cvv2;
        #endregion

        /// <summary>
        /// 缩略卡号 卡的前6后4
        /// </summary>
        public string storablePan;
    }
}
