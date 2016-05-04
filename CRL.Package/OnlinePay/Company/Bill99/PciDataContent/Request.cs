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

namespace CRL.Package.OnlinePay.Company.Bill99.PciDataContent
{
    //PCI存储
    public class Request : PCIBase
    {
        public override string InterfacePath
        {
            get
            {
                return "/cnp/pci_store";
            }
        }
        /// <summary>
        /// 持卡人姓名
        /// </summary>
        public string cardHolderName;
        /// <summary>
        /// 证件类型
        /// </summary>
        public string idType;
        /// <summary>
        /// 持卡人证件号码
        /// </summary>
        public string cardHolderId;
        /// <summary>
        /// //卡号
        /// </summary>
        public string pan;
        /// <summary>
        /// 银行代码
        /// </summary>
        public string bankId;
        /// <summary>
        /// 卡有效期
        /// </summary>
        public string expiredDate;
        /// <summary>
        /// 手机号码
        /// </summary>
        public string phoneNO;
    }
}
