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
        ParameCollection _newExpressionParame;
        ParameCollection newExpressionParame
        {
            get
            {
                _newExpressionParame = _newExpressionParame ?? new ParameCollection();
                return _newExpressionParame;
            }
            set
            {
                _newExpressionParame = _newExpressionParame ?? new ParameCollection();
                _newExpressionParame = value;
            }
        }
        internal string GetQueryFieldsString(IEnumerable<Attribute.FieldMapping> fields)
        {
            return string.Join(",", fields.Select(b => b.QueryFull));
            //var sb = "";
            //foreach (var a in fields)
            //{
            //    sb += string.Format("{0},", a.QueryFull);
            //}
            //sb.Remove(sb.Length - 1, 1);
            //return sb.Substring(0, sb.Length - 1);
        }
        #region 筛选缓存
        internal class SelectFieldInfo
        {
            public SelectFieldInfo(List<Attribute.FieldMapping> _fields, ParameCollection _parame)
            {
                parame = _parame;
                mapping = _fields;
                //mapping = _fields.FindAll(b => !b.WithTablePrefix);
                //queryFieldString = _queryFieldString;
            }
            string queryFieldString;
            public List<Attribute.FieldMapping> mapping;
            public ParameCollection parame;
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
        //internal List<Attribute.FieldMapping> _CurrentAppendSelectField = new List<Attribute.FieldMapping>();
        internal SelectFieldInfo GetSelectField(bool isSelect, Expression expressionBody, bool withTablePrefix, params Type[] types)
        {
            SelectFieldInfo item;
            //if (isSelect && SettingConfig.UseLambdaCache)
            //{
            //    var cacheKey = GetSelectFieldCacheKey(isSelect, expressionBody, withTablePrefix, types);
            //    var cache = !string.IsNullOrEmpty(cacheKey);
            //    //cache = false;
            //    if (cache)
            //    {
            //        #region cache
            //        var a = _GetSelectFieldCache.TryGetValue(cacheKey, out item);
            //        if (a)
            //        {
            //            if (expressionBody is NewExpression)
            //            {
            //                var newExp = expressionBody as NewExpression;
            //                foreach (var kv in item.parame)
            //                {
            //                    var v2 = Convert.ToInt32(kv.Value);
            //                    var exp = newExp.Arguments[v2];
            //                    if (exp is MethodCallExpression)
            //                    {
            //                        var mExp = exp as MethodCallExpression;
            //                        if (mExp.Arguments.Count > 0)
            //                        {
            //                            string mName;
            //                            getSelectMethodCall(exp, out mName, 0, false);//转换为参数
            //                        }
            //                    }
            //                    else
            //                    {
            //                        var obj = ConstantValueVisitor.GetParameExpressionValue(exp);
            //                        __Visitor.AddParame(kv.Key, obj);
            //                    }
            //                }
            //            }
            //            return item;
            //        }
            //        else
            //        {
            //            item = _GetSelectField(isSelect, expressionBody, withTablePrefix, types);
            //            //item.expression = expressionBody;
            //            _GetSelectFieldCache[cacheKey] = item;
            //            return item;
            //        }
            //        #endregion
            //    }
            //}
            item = _GetSelectField(isSelect, expressionBody, withTablePrefix, types);
            return item;
        }
        string GetSelectFieldCacheKey(bool isSelect, Expression expressionBody, bool withTablePrefix, params Type[] types)
        {
            string cacheKey = "";
            if (expressionBody is ParameterExpression)//选择所有字段 b=>b
            {
                cacheKey = "objSelect_" + expressionBody.Type;
            }
            else if (expressionBody is NewExpression)//按匿名对象
            {
                string prex = string.Join("-", __Prefixs.Keys);
                //todo expressionBody.ToString()比较占内存
                cacheKey = prex + string.Join("-", types.Select(b => b.Name)) + expressionBody.ToString() + isSelect + withTablePrefix;
                //cacheKey = CoreHelper.StringHelper.EncryptMD5(cacheKey);
                cacheKey = cacheKey.GetHashCode().ToString();
            }
            else if (expressionBody is MemberExpression)//b=>b.Id
            {
                var mExp = expressionBody as MemberExpression;
                cacheKey = string.Format("objItemSelect_{0}.{1}", mExp.Expression.Type, mExp.Member.Name);
            }
            return cacheKey;
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
        SelectFieldInfo _GetSelectField(bool isSelect, Expression expressionBody, bool withTablePrefix, params Type[] types)
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
            var resultFields = new List<Attribute.FieldMapping>();
            if (expressionBody is ParameterExpression)//选择所有字段
            {
                foreach (var item in allFields[expressionBody.Type].Values)
                {
                    var item2 = item.GetFieldMapping(__DBAdapter, GetPrefix(item.ModelType), false, "");
                    resultFields.Add(item2);
                }
                var selectFieldItem = new SelectFieldInfo(resultFields, new ParameCollection()/*, GetQueryFieldsString(resultFields)*/);
                return selectFieldItem;
            }
            else if (expressionBody is NewExpression)//按匿名对象
            {
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
                        var f = new Attribute.FieldAttribute() { ModelType = __MainType, MemberName = memberName };
                        var f2 = f.GetFieldMapping(__DBAdapter, "", withTablePrefix, memberName, methodQuery);
                        f2.MethodName = methodCallExpression.Method.Name;
                        resultFields.Add(f2);
                    }
                    else if (item is BinaryExpression)
                    {
                        var field = getSeletctBinary(item);
                        var f = new Attribute.FieldAttribute() { ModelType = __MainType, MemberName = "" };
                        var f2 = f.GetFieldMapping(__DBAdapter, "", withTablePrefix, memberName, field);
                        resultFields.Add(f2);
                    }
                    else if (item is ConstantExpression)//常量
                    {
                        var constantExpression = item as ConstantExpression;
                        var f = new Attribute.FieldAttribute() { ModelType = __MainType, MemberName = "" };
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
                            string parName = "@par" + i;
                            newExpressionParame.Add(parName, i);
                            var obj = ConstantValueVisitor.GetParameExpressionValue(item);
                            var f2 = new Attribute.FieldAttribute() { ModelType = __MainType };
                            var f3 = f2.GetFieldMapping(__DBAdapter, "", withTablePrefix, memberName, parName);
                            //f2.FieldQuery = new Attribute.FieldQuery() { MemberName = memberName, FieldName = parName, MethodName = "" };
                            resultFields.Add(f3);
                            __Visitor.AddParame(parName, obj);
                            continue;
                        }
                        else if (memberExpression.Member.ReflectedType.Name.StartsWith("<>f__AnonymousType"))
                        {
                            //按匿名对象属性,视图关联时用
                            var f2 = new Attribute.FieldAttribute() { MemberName = memberExpression.Member.Name };
                            var f3 = f2.GetFieldMapping(__DBAdapter, GetPrefix(memberExpression.Expression.Type), withTablePrefix, memberName);
                            resultFields.Add(f3);
                            continue;

                        }
                        //按属性
                        if (!allFields[memberExpression.Expression.Type].ContainsKey(memberExpression.Member.Name))
                        {
                            throw new CRLException("找不到可筛选的属性" + memberExpression.Member.Name + " 在" + memberExpression.Expression.Type);
                        }
                        var f = allFields[memberExpression.Expression.Type][memberExpression.Member.Name];
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
                var selectFieldItem = new SelectFieldInfo(resultFields, new ParameCollection(newExpressionParame)/*, GetQueryFieldsString(resultFields)*/);
                return selectFieldItem;
            }
            else if (expressionBody is MethodCallExpression)
            {
                #region 方法
                var method = expressionBody as MethodCallExpression;
                var f = new Attribute.FieldAttribute() { ModelType = __MainType, MemberName = "" };
                string methodMember;
                var methodQuery = getSelectMethodCall(expressionBody, out methodMember, 0);
                var f2 = f.GetFieldMapping(__DBAdapter, "", withTablePrefix, "", methodQuery);
                f2.MethodName = method.Method.Name;
                //f.FieldQuery = new Attribute.FieldQuery() { MemberName = methodMember, FieldName = methodMember, MethodName = method.Method.Name };
                resultFields.Add(f2);
                #endregion
                var selectFieldItem = new SelectFieldInfo(resultFields, new ParameCollection(newExpressionParame)/*, GetQueryFieldsString(resultFields)*/);
                return selectFieldItem;
            }
            else if (expressionBody is BinaryExpression)
            {
                var field = getSeletctBinary(expressionBody);
                var f = new Attribute.FieldAttribute() { ModelType = __MainType, MemberName = "" };
                var f2 = f.GetFieldMapping(__DBAdapter, "", withTablePrefix, "", field);
                //f.FieldQuery = new Attribute.FieldQuery() { MemberName = f.MemberName, FieldName = field, MethodName = "" };
                resultFields.Add(f2);
                var selectFieldItem = new SelectFieldInfo(resultFields, new ParameCollection()/*, GetQueryFieldsString(resultFields)*/);
                return selectFieldItem;
            }
            else if (expressionBody is ConstantExpression)
            {
                var constant = (ConstantExpression)expressionBody;
                var f = new Attribute.FieldAttribute() { ModelType = __MainType, MemberName = "" };
                var f2 = f.GetFieldMapping(__DBAdapter, "", withTablePrefix, "", constant.Value + "");
                //f.FieldQuery = new Attribute.FieldQuery() { MemberName = f.MemberName, FieldName = constant.Value + "", MethodName = "" };
                resultFields.Add(f2);
                var selectFieldItem = new SelectFieldInfo(resultFields, new ParameCollection()/*, GetQueryFieldsString(resultFields)*/);
                return selectFieldItem;
            }
            else if (expressionBody is UnaryExpression)
            {
                var unaryExpression = expressionBody as UnaryExpression;
                return _GetSelectField(false, unaryExpression.Operand, withTablePrefix, types);
            }
            else if (expressionBody is MemberExpression)//按成员
            {
                #region MemberExpression
                var mExp = (MemberExpression)expressionBody;
                if (mExp.Expression.Type.BaseType == typeof(object))
                {
                    //按匿名对象属性,视图关联时用
                    var _f = new Attribute.FieldAttribute() { MemberName = mExp.Member.Name };
                    var f3 = _f.GetFieldMapping(__DBAdapter, GetPrefix(mExp.Expression.Type), withTablePrefix, mExp.Member.Name);
                    resultFields.Add(f3);
                    return new SelectFieldInfo(resultFields, new ParameCollection()/*, GetQueryFieldsString(resultFields)*/);
                }

                if (!allFields[mExp.Expression.Type].ContainsKey(mExp.Member.Name))
                {
                    throw new CRLException("找不到可筛选的属性" + mExp.Member.Name + " 在" + mExp.Expression.Type);
                }
                var f = allFields[mExp.Expression.Type][mExp.Member.Name];
                var f2 = f.GetFieldMapping(__DBAdapter, GetPrefix(f.ModelType), withTablePrefix, "");
                //f.FieldQuery = new Attribute.FieldQuery() { MemberName = f.MemberName, FieldName = f.MapingName, MethodName = "" };
                resultFields.Add(f2);
                return new SelectFieldInfo(resultFields, new ParameCollection()/*, GetQueryFieldsString(resultFields)*/);
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
                if (remArgsIndex)
                {
                    newExpressionParame.Add(parName, argsIndex);
                }
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
            if (remArgsIndex)
            {
                newExpressionParame[par] = argsIndex;
            }
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
