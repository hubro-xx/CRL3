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
using System.Threading.Tasks;

namespace CRL.LambdaQuery.CRLExpression
{
    public class CRLQueryExpression
    {
        /// <summary>
        /// 对象类型,FullName
        /// </summary>
        public string Type
        {
            get;
            set;
        }
        public CRLExpression Expression
        {
            get;
            set;
        }
        /// <summary>
        /// 分页用,每页大小
        /// </summary>
        public int PageSize
        {
            get;
            set;
        }
        /// <summary>
        /// 分页用,页索引
        /// </summary>
        public int PageIndex
        {
            get;
            set;
        }
        public string ToJson()
        {
            return CoreHelper.StringHelper.SerializerToJson(this);
        }
        /// <summary>
        /// CRLQueryExpression json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static CRLQueryExpression FromJson(string json)
        {
            var result = (CRLQueryExpression)CoreHelper.StringHelper.SerializerFromJSON(System.Text.Encoding.UTF8.GetBytes(json), typeof(CRLQueryExpression));
            return result;
        }
    }
    
}
