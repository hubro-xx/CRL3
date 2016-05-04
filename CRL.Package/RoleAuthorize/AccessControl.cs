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
    /// 访问控制
    /// </summary>
    public sealed class AccessControl:IModelBase
    {
        protected override System.Collections.IList GetInitData()
        {
            return base.GetInitData();
        }
        /// <summary>
        /// 菜单
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string MenuCode
        {
            get;
            set;
        }
        public bool Que
        {
            get;
            set;
        }
        /// <summary>
        /// 角色类型,组还是用户
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public RoleType RoleType
        {
            get;
            set;
        }
        /// <summary>
        /// 角色或用户
        /// </summary>
        public int Role
        {
            get;
            set;
        }
        /// <summary>
        /// 系统ID
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int SystemTypeId
        {
            get;
            set;
        }
    }
}
