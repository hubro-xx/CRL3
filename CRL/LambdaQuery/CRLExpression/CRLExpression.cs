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
using System.Linq.Expressions;
using System.Text;

namespace CRL.LambdaQuery.CRLExpression
{
    /// <summary>
    /// CRLExpression节点
    /// </summary>
    public class CRLExpression
    {
        public override string ToString()
        {
            return CoreHelper.StringHelper.SerializerToJson(this);
        }
        /// <summary>
        /// 左节点
        /// </summary>
        public CRLExpression Left
        {
            get;
            set;
        }
        public Type MemberType
        {
            get;
            set;
        }
        /// <summary>
        /// 右节点
        /// </summary>
        public CRLExpression Right
        {
            get;
            set;
        }
        /// <summary>
        /// 节点类型
        /// </summary>
        public CRLExpressionType Type
        {
            get;
            set;
        }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data
        {
            get;
            set;
        }
        [System.Xml.Serialization.XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string DataParamed;

        /// <summary>
        /// 左右操作类型
        /// </summary>
        public string ExpType
        {
            get;
            set;
        }


        [System.Xml.Serialization.XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string SqlOut
        {
            get;
            set;
        }
        [System.Xml.Serialization.XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public bool IsConstantValue;
    }
}
