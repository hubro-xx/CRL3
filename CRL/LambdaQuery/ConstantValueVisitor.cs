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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery
{
    internal class ConstantValueVisitor
    {
        public static object GetParameExpressionValue(Expression expression)
        {
            bool isConstant;
            return GetParameExpressionValue(expression, out isConstant);
        }
        public static object GetParameExpressionValue(Expression expression,out bool isConstant)
        {
            isConstant = false;
            //只能处理常量
            if (expression is ConstantExpression)
            {
                isConstant = true;
                ConstantExpression cExp = (ConstantExpression)expression;
                return cExp.Value;
            }
            else if (expression is MemberExpression)//按属性访问
            {
                var m = expression as MemberExpression;
                if (m.Expression != null)
                {
                    if (m.Expression.NodeType == ExpressionType.Parameter)
                    {
                        string name = m.Member.Name;
                        var filed2 = TypeCache.GetProperties(m.Expression.Type, true)[name];
                        //return new ExpressionValueObj { Value = FormatFieldPrefix(m.Expression.Type, filed2.MapingName), IsMember = true };
                        return filed2.MapingName;
                    }
                    else
                    {
                        return GetMemberExpressionValue(m, out isConstant);
                    }
                }
                return GetMemberExpressionValue(m, out isConstant);
            }
            //按编译
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }
        public static object GetMemberExpressionValue(Expression exp, out bool isConstant)
        {
            isConstant = false;
            if (exp.NodeType == ExpressionType.Constant)
            {
                isConstant = true;
                return ((ConstantExpression)exp).Value;
            }
            if (exp.NodeType == ExpressionType.MemberAccess)
            {
                var mExp = (MemberExpression)exp;
                object instance = null;
                if (mExp.Expression != null)
                {
                    instance = GetMemberExpressionValue(mExp.Expression, out isConstant);
                    //字段属属性都按变量
                    isConstant = false;
                    if (instance == null)
                    {
                        throw new ArgumentNullException(exp.ToString());
                    }
                }

                if (mExp.Member.MemberType == MemberTypes.Field)
                {
                    return ((FieldInfo)mExp.Member).GetValue(instance);
                }
                else if (mExp.Member.MemberType == MemberTypes.Property)
                {
                    return ((PropertyInfo)mExp.Member).GetValue(instance, null);
                }
                throw new CRLException("未能解析" + mExp.Member.MemberType);
            }
            throw new CRLException("未能解析" + exp.NodeType);
        }
    }

}
