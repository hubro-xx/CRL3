using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTest.Code
{
    public class MemberManage : CRL.BaseProvider<Member>
    {

        /// <summary>
        /// 实例访问入口
        /// </summary>
        public static MemberManage Instance
        {
            get { return new MemberManage(); }
        }
    }
}