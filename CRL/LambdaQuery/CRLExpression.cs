using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CRL.LambdaQuery
{

    public enum CRLExpressionType
    {
        Tree,
        Binary,
        Name,
        Value,
        MethodCall
    }
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
        public int PageSize
        {
            get;
            set;
        }
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
    
    public class CRLExpression
    {
        public override string ToString()
        {
            return CoreHelper.StringHelper.SerializerToJson(this);
        }

        public CRLExpression Left
        {
            get;
            set;
        }
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
        /// <summary>
        /// 左右操作类型
        /// </summary>
        public string ExpressionType;
    }
}
