/**
* CRL 快速开发框架 V4.5
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL
{
    /// <summary>
    /// 不区分大小的字典
    /// </summary>
    public class IgnoreCaseDictionary<T> : Dictionary<string, T>
    {
        public IgnoreCaseDictionary() : base(StringComparer.OrdinalIgnoreCase)
        {

        }
        public IgnoreCaseDictionary(IDictionary<string, T> dic)
            : base(dic, StringComparer.OrdinalIgnoreCase)
        {
        }
        
    }

    /// <summary>
    /// 键值的集合,不区分大小写
    /// 如果不需要以参数形式处理,名称前加上$ 如 c2["$SoldCount"]="SoldCount+" + num;
    /// </summary>
    public class ParameCollection : IgnoreCaseDictionary<object>
    {
        public ParameCollection()
        {
        }
        public ParameCollection(IDictionary<string, object> dic)
            : base(dic)
        {
        }
    }
}
