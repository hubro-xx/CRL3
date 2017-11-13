using CoreHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery
{
    public abstract partial class LambdaQueryBase
    {
        #region 解析选择的字段
        internal string GetQueryFieldsString(IEnumerable<Attribute.FieldMapping> fields)
        {
            return string.Join(",", fields.Select(b => b.QueryFull));
        }
        #region 筛选缓存
        internal class SelectFieldInfo
        {
            public SelectFieldInfo(List<Attribute.FieldMapping> _fields)
            {
                mapping = _fields;
            }
            public SelectFieldInfo(Attribute.FieldMapping field)
            {
                mapping = mapping ?? new List<Attribute.FieldMapping>();
                mapping.Add(field);
            }
            string queryFieldString;
            public List<Attribute.FieldMapping> mapping;
            //ParameCollection parame;
            public void Merge(SelectFieldInfo item)
            {
                mapping.AddRange(item.mapping);
            }
            public void CleanQueryFieldString()
            {
                queryFieldString = null;
            }
            public string GetQueryFieldString()
            {
                if (string.IsNullOrEmpty(queryFieldString))
                {
                    queryFieldString = string.Join(",", mapping.Select(b => b.QueryFull));
                }
                return queryFieldString;
            }

            public SelectFieldInfo Clone()
            {
                var obj= MemberwiseClone() as SelectFieldInfo;
                obj.mapping = new List<Attribute.FieldMapping>(obj.mapping);
                return obj;
            }
            //public Expression expression;
        }
        //internal static Dictionary<string, SelectFieldInfo> _GetSelectFieldCache = new Dictionary<string, SelectFieldInfo>();
        internal SelectFieldInfo _CurrentSelectFieldCache
        {
            get; private set;
        }
        internal void SetSelectFiled(SelectFieldInfo info, bool overide = false)
        {
            if (_CurrentSelectFieldCache == null || overide)
            {
                _CurrentSelectFieldCache = info;
            }
            else
            {
                _CurrentSelectFieldCache.Merge(info);
            }
        }
        #endregion
        /// <summary>
        /// 解析选择的字段
        /// </summary>
        /// <param name="isSelect">查询字段时按属性名生成别名</param>
        /// <param name="expressionBody"></param>
        /// <param name="withTablePrefix">是否生按表生成前辍,关联时用 如Table__Name</param>
        /// <param name="types"></param>
        /// <returns></returns>
        internal SelectFieldInfo GetSelectField(bool isSelect, Expression expressionBody, bool withTablePrefix, params Type[] types)
        {
            var allFields = new Dictionary<Type, IgnoreCaseDictionary<Attribute.FieldAttribute>>();
            allFields.Add(__MainType, TypeCache.GetProperties(__MainType, true));
            foreach (var t in types)
            {
                if (!allFields.ContainsKey(t))
                {
                    allFields.Add(t, TypeCache.GetProperties(t, true));
                }
            }
    
            if (expressionBody is ParameterExpression)//选择所有字段
            {
                var resultFields = new List<Attribute.FieldMapping>();
                foreach (var item in allFields[expressionBody.Type].Values)
                {
                    var item2 = item.GetFieldMapping(__DBAdapter, GetPrefix(item.ModelType), false, "");
                    resultFields.Add(item2);
                }
                var selectFieldItem = new SelectFieldInfo(resultFields);
                return selectFieldItem;
            }
            else if (expressionBody is NewExpression)//按匿名对象
            {
                var resultFields = new List<Attribute.FieldMapping>();
                #region 按匿名对象

                var newExpression = expressionBody as NewExpression;
                for (int i = 0; i < newExpression.Arguments.Count(); i++)
                {
                    var item = newExpression.Arguments[i];
                    var memberName = newExpression.Members[i].Name;
                    if (item is MethodCallExpression)//group用
                    {
                        var methodCallExpression = item as MethodCallExpression;
                        string methodMember;
                        var methodQuery = getSelectMethodCall(methodCallExpression, out methodMember, i);
                        var f = new Attribute.FieldAttribute() { ModelType = __MainType, MemberName = memberName, PropertyType = item.Type };
                        var f2 = f.GetFieldMapping(__DBAdapter, "", withTablePrefix, memberName, methodQuery);
                        f2.MethodName = methodCallExpression.Method.Name;
                        resultFields.Add(f2);
                    }
                    else if (item is BinaryExpression)
                    {
                        var field = getSeletctBinary(item);
                        var f = new Attribute.FieldAttribute() { ModelType = __MainType, MemberName = "", PropertyType = item.Type };
                        var f2 = f.GetFieldMapping(__DBAdapter, "", withTablePrefix, memberName, field);
                        resultFields.Add(f2);
                    }
                    else if (item is ConstantExpression)//常量
                    {
                        var constantExpression = item as ConstantExpression;
                        var f = new Attribute.FieldAttribute() { ModelType = __MainType, MemberName = "", PropertyType = item.Type };
                        var value = constantExpression.Value + "";
                        if (!value.IsNumber())
                        {
                            value = string.Format("'{0}'", value);
                        }
                        var f2 = f.GetFieldMapping(__DBAdapter, "", withTablePrefix, memberName, value);
                        //f.FieldQuery = new Attribute.FieldQuery() { MemberName = memberName, FieldName = value, MethodName = "" };
                        resultFields.Add(f2);
                    }
                    else if (item is MemberExpression)
                    {
                        var memberExpression = item as MemberExpression;//转换为属性访问表达式
                        if (memberExpression.Expression.NodeType == ExpressionType.Constant)
                        {
                            string parName = "@p" + i;
                            //newExpressionParame.Add(parName, i);
                            var obj = ConstantValueVisitor.GetParameExpressionValue(item);
                            var f2 = new Attribute.FieldAttribute() { ModelType = __MainType, PropertyType = item.Type };
                            var f3 = f2.GetFieldMapping(__DBAdapter, "", withTablePrefix, memberName, parName);
                            //f2.FieldQuery = new Attribute.FieldQuery() { MemberName = memberName, FieldName = parName, MethodName = "" };
                            resultFields.Add(f3);
                            __Visitor.AddParame(parName, obj);
                            continue;
                        }
                        else if (memberExpression.Member.ReflectedType.Name.StartsWith("<>f__AnonymousType"))
                        {
                            //按匿名对象属性,视图关联时用
                            var f2 = new Attribute.FieldAttribute() { MemberName = memberExpression.Member.Name, PropertyType = item.Type };
                            var f3 = f2.GetFieldMapping(__DBAdapter, GetPrefix(memberExpression.Expression.Type), withTablePrefix, memberName);
                            resultFields.Add(f3);
                            continue;

                        }
                        //按属性
                        Attribute.FieldAttribute f;
                        var a2 = allFields[memberExpression.Expression.Type].TryGetValue(memberExpression.Member.Name, out f);
                        if (!a2)
                        {
                            throw new CRLException("找不到可筛选的属性" + memberExpression.Member.Name + " 在" + memberExpression.Expression.Type);
                        }
                        Attribute.FieldMapping _f2;
                        if (memberName != memberExpression.Member.Name)//按有别名算
                        {
                            _f2 = f.GetFieldMapping(__DBAdapter, GetPrefix(f.ModelType), withTablePrefix, memberName);
                        }
                        else
                        {
                            //字段名和属性名不一样时才生成别名
                            //todo 属性别名不一样时,查询应返回属性名
                            string fieldName = "";
                            if (isSelect)//查询字段时按属性名生成别名
                            {
                                if (!string.IsNullOrEmpty(f.MapingName))
                                {
                                    fieldName = f.MemberName;
                                }
                            }
                            _f2 = f.GetFieldMapping(__DBAdapter, GetPrefix(f.ModelType), withTablePrefix, fieldName);
                        }
                        //f.FieldQuery = new Attribute.FieldQuery() { MemberName = memberName, FieldName = f.MapingName, MethodName = "" };
                        resultFields.Add(_f2);
                    }
                    else
                    {
                        throw new CRLException("不支持此语法解析:" + item);
                    }
                }
                #endregion
                var selectFieldItem = new SelectFieldInfo(resultFields);
                return selectFieldItem;
            }
            else if (expressionBody is MethodCallExpression)
            {
                #region 方法
                var method = expressionBody as MethodCallExpression;
                var f = new Attribute.FieldAttribute() { ModelType = __MainType, MemberName = "", PropertyType = expressionBody.Type };
                string methodMember;
                var methodQuery = getSelectMethodCall(expressionBody, out methodMember, 0);
                var f2 = f.GetFieldMapping(__DBAdapter, "", withTablePrefix, "", methodQuery);
                f2.MethodName = method.Method.Name;
                #endregion
                var selectFieldItem = new SelectFieldInfo(f2);
                return selectFieldItem;
            }
            else if (expressionBody is BinaryExpression)
            {
                var field = getSeletctBinary(expressionBody);
                var f = new Attribute.FieldAttribute() { ModelType = __MainType, MemberName = "", PropertyType = expressionBody.Type };
                var f2 = f.GetFieldMapping(__DBAdapter, "", withTablePrefix, "", field);
                var selectFieldItem = new SelectFieldInfo(f2);
                return selectFieldItem;
            }
            else if (expressionBody is ConstantExpression)
            {
                var constant = (ConstantExpression)expressionBody;
                var f = new Attribute.FieldAttribute() { ModelType = __MainType, MemberName = "", PropertyType = expressionBody.Type };
                var f2 = f.GetFieldMapping(__DBAdapter, "", withTablePrefix, "", constant.Value + "");
                var selectFieldItem = new SelectFieldInfo(f2);
                return selectFieldItem;
            }
            else if (expressionBody is UnaryExpression)
            {
                var unaryExpression = expressionBody as UnaryExpression;
                return GetSelectField(false, unaryExpression.Operand, withTablePrefix, types);
            }
            else if (expressionBody is MemberExpression)//按成员
            {
                #region MemberExpression
                var mExp = (MemberExpression)expressionBody;
                if (mExp.Expression.Type.BaseType == typeof(object))
                {
                    //按匿名对象属性,视图关联时用
                    var _f = new Attribute.FieldAttribute() { MemberName = mExp.Member.Name, PropertyType = mExp.Type };
                    var f3 = _f.GetFieldMapping(__DBAdapter, GetPrefix(mExp.Expression.Type), withTablePrefix, mExp.Member.Name);
                    return new SelectFieldInfo(f3);
                }
                CRL.Attribute.FieldAttribute f;
                var a = allFields[mExp.Expression.Type].TryGetValue(mExp.Member.Name, out f);
                if (!a)
                {
                    throw new CRLException("找不到可筛选的属性" + mExp.Member.Name + " 在" + mExp.Expression.Type);
                }
                var f2 = f.GetFieldMapping(__DBAdapter, GetPrefix(f.ModelType), withTablePrefix, "");
                return new SelectFieldInfo(f2);
                #endregion
            }
            else
            {
                throw new CRLException("不支持此语法解析:" + expressionBody);
            }
        }
        /// <summary>
        /// 返回方法调用拼接
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="memberName"></param>
        /// <param name="argsIndex"></param>
        /// <param name="remArgsIndex"></param>
        /// <returns></returns>
        string getSelectMethodCall(Expression expression, out string memberName, int argsIndex, bool remArgsIndex = true)
        {
            var cRLExpression = __Visitor.RouteExpressionHandler(expression);
            memberName = "";
            if (cRLExpression.Type == CRLExpression.CRLExpressionType.Value)
            {
                //值类型返回参数值
                var parName = "@cons" + __DbContext.parIndex;
                __DbContext.parIndex += 1;
                __Visitor.AddParame(parName, cRLExpression.Data);
                //if (remArgsIndex)
                //{
                //    newExpressionParame.Add(parName, argsIndex);
                //}
                return parName;
            }
            var methodCallObj = cRLExpression.Data as CRLExpression.MethodCallObj;
            memberName = methodCallObj.MethodName;

            var dic = MethodAnalyze.GetMethos(__DBAdapter);
            if (!dic.ContainsKey(methodCallObj.MethodName))
            {
                throw new CRLException("LambdaQuery不支持扩展方法" + methodCallObj.MemberQueryName + "." + methodCallObj.MethodName);
            }
            int newParIndex = __DbContext.parIndex;

            var par = dic[methodCallObj.MethodName](methodCallObj, ref newParIndex, __Visitor.AddParame);
            __DbContext.parIndex = newParIndex;
            //if (remArgsIndex)
            //{
            //    newExpressionParame[par] = argsIndex;
            //}
            return par;
            //return string.Format("{0}({1})", methodName, methodField);
        }
        /// <summary>
        /// 返回二元运算调用
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        string getSeletctBinary(Expression expression)
        {
            var str = __Visitor.RouteExpressionHandler(expression).SqlOut;
            return str;
        }
        #endregion
    }
}
