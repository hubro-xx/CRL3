using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.RoleAuthorize
{
    [Attribute.Table(TableName = "Department")]
    public class Department:Category.Category
    {
        public override System.Collections.IList GetInitData()
        {
            var list = new List<Department>();
            list.Add(new Department() { Name = "部门一", SequenceCode = "01", DataType = 0 });
            return list;
        }
    }
}
