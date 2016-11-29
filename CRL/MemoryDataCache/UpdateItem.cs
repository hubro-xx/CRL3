/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CoreHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.MemoryDataCache
{
    /// <summary>
    /// 更新的项
    /// </summary>
    class UpdateItem
    {
        public string Key;
        public string TableName;
        public DBHelper DBHelper;
        public Dictionary<string, object> Params;
        public DateTime UpdateTime;
        public Type Type;
        public IEnumerable<Attribute.FieldMapping> Mapping;
    }
}
