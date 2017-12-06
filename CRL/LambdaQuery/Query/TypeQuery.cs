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
using System.Threading.Tasks;

namespace CRL.LambdaQuery
{
    /// <summary>
    /// 表示查询的类型
    /// </summary>
    internal class TypeQuery
    {
        public TypeQuery(Type _OriginType)
        {
            OriginType = _OriginType;
            TypeQueryEnum = TypeQueryEnum.表;
        }
        public TypeQuery(Type _OriginType,string _queryName)
        {
            OriginType = _OriginType;
            TypeQueryEnum = TypeQueryEnum.查询;
            queryName2 = _queryName;
        }
        public string _GetKey()
        {
            return TypeQueryEnum.ToString() + OriginType.Name;
        }
        public override string ToString()
        {
            return _GetKey();
        }
        public override int GetHashCode()
        {
            var key = _GetKey();
            var h = key.GetHashCode();
            return h;
        }
        public override bool Equals(object obj)
        {
            var obj2 = obj as TypeQuery;
            var a= GetHashCode() == obj2.GetHashCode();
            return a;
        }
        public TypeQueryEnum TypeQueryEnum;
        public Type OriginType;
        public string queryName2;
        public string InnerQuery;
    }
    enum TypeQueryEnum
    {
        表,
        查询
    }
}
