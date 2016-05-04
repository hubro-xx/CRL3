/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using Shopping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.BLL
{
    /// <summary>
    /// 商家管理
    /// </summary>
    public class SupplierManage : CRL.Package.Person.PersonBusiness<SupplierManage, Supplier>
    {
        public static SupplierManage Instance
        {
            get { return new SupplierManage(); }
        }
    }
}
