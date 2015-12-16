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
        public override System.Collections.IList GetInitData()
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
