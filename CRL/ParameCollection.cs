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
    /// </summary>
    public class ParameCollection : IgnoreCaseDictionary<object>
    {
        
    }
    
}
