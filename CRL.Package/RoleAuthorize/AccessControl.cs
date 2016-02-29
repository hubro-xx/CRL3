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
