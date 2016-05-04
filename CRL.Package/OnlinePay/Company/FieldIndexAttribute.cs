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
