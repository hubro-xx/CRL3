using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.OnlinePay.Company
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldIndexAttribute : System.Attribute
    {
        public FieldIndexAttribute(int _index)
        {
            index = _index;
        }
        int index = 999;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }
    }
}
