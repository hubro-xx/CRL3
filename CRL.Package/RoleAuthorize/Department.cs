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
    [Attribute.Table(TableName = "Department")]
    public class Department:Category.Category
    {
        protected override System.Collections.IList GetInitData()
        {
            var list = new List<Department>();
            list.Add(new Department() { Name = "部门一", SequenceCode = "01", DataType = 0 });
            return list;
        }
    }
}
