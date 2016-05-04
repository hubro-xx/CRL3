/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.DicConfig
{
    /// <summary>
    /// 字典配置,用来保存固定值或动态值
    /// 不要继承
    /// </summary>
    public class DicConfig : IModelBase
    {
        public override string CheckData()
        {
            return "";
        }

        /// <summary>
        /// 类型
        /// </summary>
        [Attribute.Field(FieldIndexType= Attribute.FieldIndexType.非聚集,NotNull=true)]
        public string DicType
        {
            get;
            set;
        }

        /// <summary>
        /// 名称
        /// </summary>
        [Attribute.Field(FieldIndexType= Attribute.FieldIndexType.非聚集,NotNull = true)]
        public string Name
        {
            get;
            set;
        }
        [Attribute.Field(Length = 200, NotNull = true)]
        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get;
            set;
        }
        public string Remark
        {
            get;
            set;
        }
        /// <summary>
        /// 是否能更改值
        /// </summary>
        public bool CanChange
        {
            get;
            set;
        }
    }
}
