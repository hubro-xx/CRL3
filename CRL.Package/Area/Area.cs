using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.Area
{
    /// <summary>
    /// 区域
    /// </summary>
    public class Area
    {
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// 父ID
        /// </summary>
        public string ParentCode
        {
            get;
            set;
        }
        public int Level
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
