using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.Person
{
    public class SmsSendRecordManage : CRL.BaseProvider<SmsSendRecord>
    {
        public static SmsSendRecordManage Instance
        {
            get
            {
                return new SmsSendRecordManage();
            }
        }
    }
    public class SmsSendRecord : CRL.IModelBase
    {
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集)]
        public string Mobile
        {
            get;
            set;
        }
        public string Code
        {
            get;
            set;
        }
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集)]
        public string ModuleName
        {
            get;
            set;
        }
        /// <summary>
        /// 20分钟过期
        /// </summary>
        public bool Expired
        {
            get
            {
                return (DateTime.Now - AddTime).TotalMinutes > 20;
            }
        }
    }
}
