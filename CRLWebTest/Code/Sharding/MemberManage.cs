using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTest.Code.Sharding
{
    public class MemberSharding : CRL.IModel
    {
        [CRL.Attribute.Field(KeepIdentity=true)]//保持插入主键
        public int Id
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
    public class MemberManage : CRL.Sharding.BaseProvider<MemberSharding>
    {
        public static MemberManage Instance
        {
            get { return new MemberManage(); }
        }
    }
}