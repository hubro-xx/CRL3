using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.BLL.ProxyPool
{
    public class IpProxy:CRL.IModelBase
    {
        public string Address
        {
            get;
            set;
        }
        public int Port
        {
            get;
            set;
        }
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集唯一)]
        public string FullAddress
        {
            get;
            set;
        }
        public DateTime CheckTime
        {
            get;
            set;
        }
        public bool CheckResult
        {
            get;
            set;
        }
    }
    public class IpProxyManage : CRL.BaseProvider<IpProxy>
    {
        public static IpProxyManage Instance
        {
            get
            {
                return new IpProxyManage();
            }
        }
    }
}