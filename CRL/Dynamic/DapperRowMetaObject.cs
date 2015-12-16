using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CRL.Dynamic
{
    internal sealed partial class DapperRowMetaObject : System.Dynamic.DynamicMetaObject
    {
        static readonly MethodInfo getValueMethod = typeof(IDictionary<string, object>).GetProperty("Item").GetGetMethod();
        static readonly MethodInfo setValueMethod = typeof(DynamicObject).GetMethod("SetValue");

        public DapperRowMetaObject(
            System.Linq.Expressions.Expression expression,
            System.Dynamic.BindingRestrictions restrictions
            )
            : base(expression, restrictions)
        {
        }

        public DapperRowMetaObject(
            System.Linq.Expressions.Expression expression,
            System.Dynamic.BindingRestrictions restrictions,
            object value
            )
            : base(expression, restrictions, value)
        {
        }

        System.Dynamic.DynamicMetaObject CallMethod(
            MethodInfo method,
            System.Linq.Expressions.Expression[] parameters
            )
        {
            var callMethod = new System.Dynamic.DynamicMetaObject(
                System.Linq.Expressions.Expression.Call(
                    System.Linq.Expressions.Expression.Convert(Expression, LimitType),
                    method,
                    parameters),
                System.Dynamic.BindingRestrictions.GetTypeRestriction(Expression, LimitType)
                );
            return callMethod;
        }

        public override System.Dynamic.DynamicMetaObject BindGetMember(System.Dynamic.GetMemberBinder binder)
        {
            var parameters = new System.Linq.Expressions.Expression[]
									 {
										 System.Linq.Expressions.Expression.Constant(binder.Name)
									 };

            var callMethod = CallMethod(getValueMethod, parameters);

            return callMethod;
        }

        public override System.Dynamic.DynamicMetaObject BindSetMember(System.Dynamic.SetMemberBinder binder, System.Dynamic.DynamicMetaObject value)
        {
            var parameters = new System.Linq.Expressions.Expression[]
									 {
										 System.Linq.Expressions.Expression.Constant(binder.Name),
										 value.Expression,
									 };

            var callMethod = CallMethod(setValueMethod, parameters);

            return callMethod;
        }
    }
}
