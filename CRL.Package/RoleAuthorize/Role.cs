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
    /// <summary>
    /// 角色组
    /// </summary>
    [Attribute.Table( TableName="Roles")]
    public sealed class Role : IModelBase
    {
        protected override System.Collections.IList GetInitData()
        {
            var list = new List<Role>();
            list.Add(new Role() { Name = "管理员" });
            list.Add(new Role() { Name = "普通用户" });
            return list;
        }
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集唯一)]
        public string Name
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
}
