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
using System.Threading.Tasks;

namespace CRL.LambdaQuery.CRLExpression
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public enum CRLExpressionType
    {
        Tree = 1,
        Binary = 2,
        Name = 4,
        Value = 8,
        MethodCall = 16,
        MethodCallArgs = 32
    }
}
