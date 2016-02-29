using CRL.Sharding.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Sharding
{
    /// <summary>
    /// 数据定位
    /// </summary>
    public class Location
    {
        public DataBase DataBase;
        public TablePart TablePart;
        public override string ToString()
        {
            return string.Format("在库[{0}],表[{1}]", DataBase.Name, TablePart.PartName);
        }
    }
}
