using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.RoleAuthorize
{
    /// <summary>
    /// 
    /// </summary>
    public class EmployeeBusiness : Person.PersonBusiness<EmployeeBusiness, Employee>
    {
        public static EmployeeBusiness Instance
        {
            get
            {
                return new EmployeeBusiness();
            }
        }

    }
}
