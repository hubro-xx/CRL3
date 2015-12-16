using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.Account
{
    /// <summary>
    /// 锁定记录
    /// </summary>
    [Attribute.Table(TableName = "LockRecord")]
    public class LockRecord : IModelBase
    {
        public int UserId
        {
            get;
            set;
        }
        public override string CheckData()
        {
            return "";
        }
        public int AccountId
        {
            get;
            set;
        }
        public decimal Amount
        {
            get;
            set;
        }
        [Attribute.Field(Length = 500)]
        public string Remark
        {
            get;
            set;
        }
        /// <summary>
        /// 是否处理过
        /// </summary>
        public bool Checked
        {
            get;
            set;
        } 
    }
}
