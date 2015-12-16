using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.RoleAuthorize
{
    public class RoleBusiness : BaseProvider<Role>
    {
        public IEnumerable<Role> allCache
        {
            get
            {
                return AllCache;
            }
        }
        public static RoleBusiness Instance
        {
            get { return new RoleBusiness(); }
        }

    }
}
