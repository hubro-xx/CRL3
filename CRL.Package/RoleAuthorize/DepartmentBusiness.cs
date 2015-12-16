using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.RoleAuthorize
{
    public class DepartmentBusiness : Category.CategoryBusiness<DepartmentBusiness,Department>
    {
        public static DepartmentBusiness Instance
        {
            get { return new DepartmentBusiness(); }
        }
    }
}
