using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTest.Code
{
    public class ClassGuid:CRL.IModel
    {
        [CRL.Attribute.Field(IsPrimaryKey=true,KeepIdentity=true)]
        public Guid id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
    }
    public class ClassGuidManage : CRL.BaseProvider<ClassGuid>
    {
        public static ClassGuidManage Instance
        {
            get { return new ClassGuidManage(); }
        }
    }
}