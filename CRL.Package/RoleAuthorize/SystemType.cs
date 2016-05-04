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
    public sealed class SystemType : IModelBase
    {
        protected override System.Collections.IList GetInitData()
        {
            var list = new List<SystemType>();
            list.Add(new SystemType() { Name = "默认系统" });
            return list;
        }
        public string Name
        {
            get;
            set;
        }
    }
}
