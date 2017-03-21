/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;

namespace CRL.Attribute
{

    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : System.Attribute
    {
        public override string ToString()
        {
            return TableName;
        }
        /// <summary>
        /// 表名
        /// MongoDB不支持表和字段别名
        /// </summary>
        public string TableName
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get;
            set;
        }
        /// <summary>
        /// 默认排序 如Id Desc
        /// </summary>
        public string DefaultSort
        {
            get;
            set;
        }
        /// <summary>
        /// 自增主键
        /// </summary>
        internal FieldAttribute PrimaryKey
        {
            get;
            set;
        }
        public FieldAttribute GetPrimaryKey()
        {
            return PrimaryKey;
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public Type Type { get; set; }
        
        /// <summary>
        /// 所有字段
        /// </summary>
        internal List<FieldAttribute> Fields = new List<FieldAttribute>();
        /// <summary>
        /// 只存基本数据库字段
        /// </summary>
        internal IgnoreCaseDictionary<FieldAttribute> FieldsDic = new IgnoreCaseDictionary<FieldAttribute>();
    }
}
