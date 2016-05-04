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

namespace CRL.Package.RoleAuthorize
{
    public class SystemTypeBusiness : BaseProvider<SystemType>
    {
        public static SystemTypeBusiness Instance
        {
            get { return new SystemTypeBusiness(); }
        }

        //static List<SystemType> systemTypes;
        public List<SystemType> SystemTypes
        {
            get
            {
                //if (systemTypes == null)
                //{
                    var systemTypes = QueryList().OrderByDescending(b=>b.Id).ToList();
                //}
                return systemTypes;
            }
        }
    }
}
