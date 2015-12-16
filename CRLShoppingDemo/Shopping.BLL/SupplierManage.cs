using Shopping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.BLL
{
    /// <summary>
    /// 商家管理
    /// </summary>
    public class SupplierManage : CRL.Package.Person.PersonBusiness<SupplierManage, Supplier>
    {
        public static SupplierManage Instance
        {
            get { return new SupplierManage(); }
        }
    }
}
