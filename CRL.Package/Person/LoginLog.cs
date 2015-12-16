using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.Person
{
    public class LoginLog : CRL.IModelBase
    {
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int UserId
        {
            get;
            set;
        }
        public string UserName
        {
            get;
            set;
        }
        /// <summary>
        /// 来源
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string Source
        {
            get;
            set;
        }
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string UserType
        {
            get;
            set;
        }
        public string RemoteIp
        {
            get;
            set;
        }
        [Attribute.Field(Length=200)]
        public string Browser
        {
            get;
            set;
        }
    }
}
