using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;

namespace CRL
{
    /// <summary>
    /// 键值的集合,不区分大小写
    /// 如果不需要以参数形式处理,名称前加上$ 如 c2["$SoldCount"]="SoldCount+" + num;
    /// 分页参数仅在分页时才会用到,查询缓存参数则相反
    /// </summary>
    public class ParameCollection : IgnoreCaseDictionary<object>
    {
        
    }
    
}
