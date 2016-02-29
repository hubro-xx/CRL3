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
        Tree,
        Binary,
        Name,
        Value,
        MethodCall
    }
}
