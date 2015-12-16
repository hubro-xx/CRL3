using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.RoleAuthorize
{
    public sealed class SystemType : IModelBase
    {
        public override System.Collections.IList GetInitData()
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
