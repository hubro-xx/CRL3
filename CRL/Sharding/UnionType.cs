using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Sharding
{
    /// <summary>
    /// 分表联合查询时,关联方式
    /// </summary>
    public enum UnionType
    {
        None,
        Union,
        UnionAll
    }
}
