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
using System.Threading.Tasks;
using CoreHelper;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.RegularExpressions;
namespace CRL.LambdaQuery
{
    public abstract class LambdaQueryBase
    {
        /// <summary>
        /// 当前类型
        /// </summary>
        internal Type __MainType;
        internal ExpressionVisitor __Visitor;
        internal bool __FromDbContext = false;
        /// <summary>
        /// 处理后的查询参数
        /// </summary>
        internal Dictionary<string, object> QueryParames
        {
            get
            {
                return __Visitor.QueryParames;
            }
        }
        /// <summary>
        /// 查询字段是否需要加上前辍,如t1.Id
        /// </summary>
        internal bool __UseTableAliasesName = true;
        /// <summary>
        /// 是否编译为存储过程
        /// </summary>
        internal bool __CompileSp;
        /// <summary>
        /// 获取记录条数
        /// </summary>
        public int TakeNum = 0;

        /// <summary>
        /// 分页索引,要分页,设为大于0
        /// </summary>
        public int SkipPage = 0;
        ///// <summary>
        ///// 查询的字段
        ///// </summary>
        //internal List<Attribute.FieldAttribute> __QueryFields = new List<CRL.Attribute.FieldAttribute>();
        /// <summary>
        /// group字段
        /// </summary>
        internal List<Attribute.FieldAttribute> __GroupFields = new List<CRL.Attribute.FieldAttribute>();
        internal Dictionary<TypeQuery, string> __Relations = new Dictionary<TypeQuery, string>();
        internal DbContext __DbContext;
        internal DBAdapter.DBAdapterBase __DBAdapter;
        internal bool __DistinctFields = false;
        /// <summary>
        /// 对象转换时间
        /// </summary>
        public double MapingTime = 0;
        /// <summary>
        /// 查询返回的总行数
        /// </summary>
        public int RowCount = 0;
        /// <summary>
        /// 缓存查询过期时间
        /// </summary>
        internal int __ExpireMinute = 0;

        /// <summary>
        /// 语法解析时间
        /// </summary>
        public double AnalyticalTime = 0;
        /// <summary>
        /// 语句执行时间
        /// </summary>
        public double ExecuteTime;
        /// <summary>
        /// 排序
        /// </summary>
        internal string __QueryOrderBy = "";
        /// <summary>
        /// 填充参数
        /// </summary>
        /// <param name="db"></param>
        internal void FillParames(AbsDBExtend db)
        {
            //db.ClearParams();
            foreach (var n in QueryParames)
            {
                db.SetParam(n.Key, n.Value);
            }
        }
        internal bool __WithNoLock = true;
        //internal bool __QueryAllField = false;
        #region 解析选择的字段
        internal string GetQueryFieldsString(List<Attribute.FieldAttribute> fields)
        {
            var sb = new StringBuilder();
            foreach (Attribute.FieldAttribute a in fields)
            {
                if (a.FieldType == Attribute.FieldType.关联字段)
                {
                    #region 关联约束
                    if (a.FieldType == Attribute.FieldType.关联字段 && a.ConstraintType == null)//虚拟字段,没有设置关联类型
                    {
                        throw new CRLException(string.Format("需指定关联类型:{0}.{1}.Attribute.Field.ConstraintType", __MainType, a.MemberName));
                    }
                    if (string.IsNullOrEmpty(a.ConstraintField))//约束为空
                    {
                        continue;
                    }
                    var arry = a.ConstraintField.Replace("$", "").Split('=');
                    string leftField = GetPrefix() + arry[0];
                    var innerType = a.ConstraintType;
                    //TypeCache.SetDBAdapterCache(innerType,dBAdapter);
                    string rightField = GetPrefix(innerType) + arry[1];
                    string condition = string.Format("{0}={1}", leftField, rightField);
                    if (!string.IsNullOrEmpty(a.Constraint))
                    {
                        a.Constraint = Regex.Replace(a.Constraint, @"(.+?)\=", GetPrefix(innerType) + "$1=");//加上前缀
                        condition += " and " + a.Constraint;
                    }

                    var innerFields = TypeCache.GetProperties(innerType, true);

                    //var resultField = innerFields.Find(b => b.Name.ToUpper() == a.ConstraintResultField.ToUpper());
                    var resultField = innerFields[a.ConstraintResultField];
                    if (resultField == null)
                    {
                        throw new CRLException(string.Format("在类型{0}找不到 ConstraintResultField {1}", innerType, a.ConstraintResultField));
                    }
                    AddInnerRelation(new TypeQuery(innerType), JoinType.Inner, condition);
                    #endregion
                    sb.Append(string.Format("{0},", resultField.QueryFullScript));
                }
                else
                {
                    sb.Append(string.Format("{0},", a.QueryFullScript));
                }
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString(); 
        }
        #region 筛选缓存
        internal class SelectFieldInfo
        {
            public SelectFieldInfo(List<Attribute.FieldAttribute> _fields, ParameCollection _parame, string _queryFieldString)
            {
                fields = _fields;
                parame = _parame;
                mapping = fields.Where(b => !b.FieldMapping.WithTablePrefix).Select(b => b.FieldMapping);
                queryFieldString = _queryFieldString;
            }
            public string queryFieldString;
            public List<Attribute.FieldAttribute> fields;
            public IEnumerable<Attribute.FieldMapping> mapping;
            public ParameCollection parame;
            //public Expression expression;
        }
        internal static Dictionary<string, SelectFieldInfo> _GetSelectFieldCache = new Dictionary<string, SelectFieldInfo>();
        internal SelectFieldInfo _CurrentSelectFieldCache;
        internal List<Attribute.FieldAttribute> _CurrentAppendSelectField = new List<Attribute.FieldAttribute>();
        internal SelectFieldInfo GetSelectField(bool isSelect, Expression expressionBody, bool withTablePrefix, params Type[] types)
        {
            SelectFieldInfo item;
            if (isSelect)
            {
                var cacheKey = GetSelectFieldCacheKey(isSelect, expressionBody, withTablePrefix, types);
                var cache = !string.IsNullOrEmpty(cacheKey);
                //cache = false;
                if (cache)
                {
                    #region cache
                    var a = _GetSelectFieldCache.TryGetValue(cacheKey, out item);
                    if (a)
                    {
                        if (expressionBody is NewExpression)
                        {
                            var newExp = expressionBody as NewExpression;
                            foreach (var kv in item.parame)
                            {
                                var exp = newExp.Arguments[Convert.ToInt32(kv.Value)];
                                if (exp is MethodCallExpression)
                                {
                                    var mExp = exp as MethodCallExpression;
                                    if (mExp.Arguments.Count > 0)
                                    {
                                        string mName;
                                        getSelectMethodCall(exp, out mName, 0, false);//转换为参数
                                    }
                                }
                                else
                                {
                                    var obj = ConstantValueVisitor.GetParameExpressionValue(exp);
                                    __Visitor.AddParame(kv.Key, obj);
                                }
                            }
                        }
                        return item;
                    }
                    else
                    {
                        item = _GetSelectField(isSelect, expressionBody, withTablePrefix, types);
                        //item.expression = expressionBody;
                        _GetSelectFieldCache[cacheKey] = item;
                        return item;
                    }
                    #endregion
                }
            }
            item = _GetSelectField(isSelect, expressionBody, withTablePrefix, types);
            return item;
        }
        string GetSelectFieldCacheKey(bool isSelect, Expression expressionBody, bool withTablePrefix,params Type[] types)
        {
            string cacheKey = "";
            if (expressionBody is ParameterExpression)//选择所有字段
            {
                cacheKey = "objSelect_" + expressionBody.Type;
            }
            else if (expressionBody is NewExpression)//按匿名对象
            {
                string prex = string.Join("-", __Prefixs);
                cacheKey = prex + string.Join("-", types.ToList()) + expressionBody.ToString() + isSelect + withTablePrefix;
                cacheKey = CoreHelper.StringHelper.EncryptMD5(cacheKey);
            }
            return cacheKey;
        }
        ParameCollection newExpressionParame = new ParameCollection();
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
            List<Attribute.FieldAttribute> resultFields = new List<Attribute.FieldAttribute>();
            if (expressionBody is ParameterExpression)//选择所有字段
            {
                foreach (var item in allFields[expressionBody.Type].Values)
                {
                    var item2 = item.Clone();
                    item2.SetFieldQueryScript2(__DBAdapter, GetPrefix(item2.ModelType), false, "");
                    resultFields.Add(item2);
                }
                var selectFieldItem = new SelectFieldInfo(resultFields, new ParameCollection(), GetQueryFieldsString(resultFields));
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
                        var methodQuery = getSelectMethodCall(methodCallExpression, out methodMember,i);
                        var f = new Attribute.FieldAttribute() { ModelType = __MainType };
                        f.SetFieldQueryScript2(__DBAdapter, "", withTablePrefix, memberName, methodQuery);
                        f.FieldQuery = new Attribute.FieldQuery() { MemberName = memberName, FieldName = methodMember, MethodName = methodCallExpression.Method.Name };
                        resultFields.Add(f);
                    }
                    else if (item is BinaryExpression)
                    {
                        var field = getSeletctBinary(item);
                        var f = new Attribute.FieldAttribute() { ModelType = __MainType };
                        f.SetFieldQueryScript2(__DBAdapter, "", withTablePrefix, memberName, field);
                        f.FieldQuery = new Attribute.FieldQuery() { MemberName = memberName, FieldName = field, MethodName = "" };
                        resultFields.Add(f);
                    }
                    else if (item is ConstantExpression)//常量
                    {
                        var constantExpression = item as ConstantExpression;
                        var f = new Attribute.FieldAttribute() { ModelType = __MainType };
                        var value = constantExpression.Value + "";
                        if (!value.IsNumber())
                        {
                            value = string.Format("'{0}'", value);
                        }
                        f.SetFieldQueryScript2(__DBAdapter, "", withTablePrefix, memberName, value);
                        f.FieldQuery = new Attribute.FieldQuery() { MemberName = memberName, FieldName = value, MethodName = "" };
                        resultFields.Add(f);
                    }
                    else if (item is MemberExpression)
                    {
                        var memberExpression = item as MemberExpression;//转换为属性访问表达式
                        if (memberExpression.Expression.NodeType== ExpressionType.Constant)
                        {
                            string parName = "@par" + i;
                            newExpressionParame.Add(parName, i);
                            var obj = ConstantValueVisitor.GetParameExpressionValue(item);
                            var f2 = new Attribute.FieldAttribute() { ModelType = __MainType };
                            f2.SetFieldQueryScript2(__DBAdapter, "", withTablePrefix, memberName, parName);
                            f2.FieldQuery = new Attribute.FieldQuery() { MemberName = memberName, FieldName = parName, MethodName = "" };
                            resultFields.Add(f2);
                            __Visitor.AddParame(parName, obj);
                            continue;
                        }
                        else if (memberExpression.Member.ReflectedType.Name.StartsWith("<>f__AnonymousType"))
                        {
                            //按匿名对象属性,视图关联时用
                            var f2 = new Attribute.FieldAttribute() { MemberName = memberExpression.Member.Name };
                            f2.SetFieldQueryScript2(__DBAdapter, GetPrefix(memberExpression.Expression.Type), withTablePrefix, memberName);
                            resultFields.Add(f2);
                            continue;

                        }
                        //按属性
                        if (!allFields[memberExpression.Expression.Type].ContainsKey(memberExpression.Member.Name))
                        {
                            throw new CRLException("找不到可筛选的属性" + memberExpression.Member.Name + " 在" + memberExpression.Expression.Type);
                        }
                        var f = allFields[memberExpression.Expression.Type][memberExpression.Member.Name].Clone();
                        if (memberName != memberExpression.Member.Name)//按有别名算
                        {
                            f.SetFieldQueryScript2(__DBAdapter, GetPrefix(f.ModelType), withTablePrefix, memberName);
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
                            f.SetFieldQueryScript2(__DBAdapter, GetPrefix(f.ModelType), withTablePrefix, fieldName);
                        }
                        f.FieldQuery = new Attribute.FieldQuery() { MemberName = memberName, FieldName = f.MapingName, MethodName = "" };
                        resultFields.Add(f);
                    }
                    else
                    {
                        throw new CRLException("不支持此语法解析:" + item);
                    }
                }
                #endregion
                var selectFieldItem = new SelectFieldInfo(resultFields, newExpressionParame, GetQueryFieldsString(resultFields));
                return selectFieldItem;
            }
            else if (expressionBody is MethodCallExpression)
            {
                #region 方法
                var method = expressionBody as MethodCallExpression;
                var f = new Attribute.FieldAttribute() { ModelType = __MainType };
                string methodMember;
                var methodQuery = getSelectMethodCall(expressionBody, out methodMember,0);
                f.SetFieldQueryScript2(__DBAdapter, "", withTablePrefix, "", methodQuery);
                f.FieldQuery = new Attribute.FieldQuery() { MemberName = methodMember, FieldName = methodMember, MethodName = method.Method.Name };
                resultFields.Add(f);
                #endregion
                var selectFieldItem = new SelectFieldInfo(resultFields, newExpressionParame, GetQueryFieldsString(resultFields));
                return selectFieldItem;
            }
            else if (expressionBody is BinaryExpression)
            {
                var field = getSeletctBinary(expressionBody);
                var f = new Attribute.FieldAttribute() { ModelType = __MainType };
                f.SetFieldQueryScript2(__DBAdapter, "", withTablePrefix, "", field);
                f.FieldQuery = new Attribute.FieldQuery() { MemberName = f.MemberName, FieldName = field, MethodName = "" };
                resultFields.Add(f);
                var selectFieldItem = new SelectFieldInfo(resultFields, new ParameCollection(), GetQueryFieldsString(resultFields));
                return selectFieldItem;
            }
            else if (expressionBody is ConstantExpression)
            {
                var constant = (ConstantExpression)expressionBody;
                var f = new Attribute.FieldAttribute() { ModelType = __MainType };
                f.SetFieldQueryScript2(__DBAdapter, "", withTablePrefix, "", constant.Value + "");
                f.FieldQuery = new Attribute.FieldQuery() { MemberName = f.MemberName, FieldName = constant.Value + "", MethodName = "" };
                resultFields.Add(f);
                var selectFieldItem = new SelectFieldInfo(resultFields, new ParameCollection(), GetQueryFieldsString(resultFields));
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
                    var f2 = new Attribute.FieldAttribute() { MemberName = mExp.Member.Name };
                    f2.SetFieldQueryScript2(__DBAdapter, GetPrefix(mExp.Expression.Type), withTablePrefix, mExp.Member.Name);
                    resultFields.Add(f2);
                    return new SelectFieldInfo(resultFields, new ParameCollection(), GetQueryFieldsString(resultFields));
                }

                if (!allFields[mExp.Expression.Type].ContainsKey(mExp.Member.Name))
                {
                    throw new CRLException("找不到可筛选的属性" + mExp.Member.Name + " 在" + mExp.Expression.Type);
                }
                var f = allFields[mExp.Expression.Type][mExp.Member.Name].Clone();
                f.SetFieldQueryScript2(__DBAdapter, GetPrefix(f.ModelType), withTablePrefix, "");
                f.FieldQuery = new Attribute.FieldQuery() { MemberName = f.MemberName, FieldName = f.MapingName, MethodName = "" };
                resultFields.Add(f);
                return new SelectFieldInfo(resultFields, new ParameCollection(), GetQueryFieldsString(resultFields));
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
        /// <returns></returns>
        string getSelectMethodCall(Expression expression, out string memberName,int argsIndex, bool remArgsIndex=true)
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

        #region 别名
        /// <summary>
        /// 别名
        /// </summary>
        internal Dictionary<Type, string> __Prefixs = new Dictionary<Type, string>();
        internal string __PrefixsAllKey;
        internal int prefixIndex = 0;
        /// <summary>
        /// 获取别名,如t1.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal string GetPrefix(Type type = null)
        {
            if (type == null)
            {
                type = __MainType;
            }
            string prefix;
            var a = __Prefixs.TryGetValue(type, out prefix);
            if (!a)
            {
                prefixIndex += 1;
                string prefix2 = string.Format("t{0}.", prefixIndex);
                if (!__UseTableAliasesName)
                {
                    prefix2 = "";
                }
                __Prefixs[type] = prefix2;
                __PrefixsAllKey = string.Join("-", __Prefixs);
                return prefix2;
            }
            return prefix;
        }
        #endregion
        internal void SelectAll()
        {
            //var all = TypeCache.GetTable(__MainType).Fields;
            //__QueryFields.Clear();
            //foreach (var item in all)
            //{
            //    var item2 = item.Clone();
            //    item2.SetFieldQueryScript2(__DBAdapter, GetPrefix(item2.ModelType), false, "");
            //    __QueryFields.Add(item2);
            //}
            var exp = Expression.Parameter(__MainType,"b");
            var fields = GetSelectField(true, exp, false, __MainType);
            _CurrentSelectFieldCache = fields;
            //__QueryFields = fields;
        }

        /// <summary>
        /// 转换为SQL条件，并提取参数
        /// </summary>
        /// <param name="expressionBody"></param>
        /// <returns></returns>
        internal CRLExpression.CRLExpression FormatExpression(Expression expressionBody)
        {
            //string condition;
            if (expressionBody == null)
                return null;
            var result = __Visitor.RouteExpressionHandler(expressionBody, firstLevel: true);
            if (string.IsNullOrEmpty(result.SqlOut))//没有构成树
            {
                bool isNullValue;
                result.SqlOut = __Visitor.DealCRLExpression(expressionBody, result, "", out isNullValue);
            }
            return result;
        }
        
        internal string FormatJoinExpression(Expression expressionBody)
        {
            string condition;
            if (expressionBody == null)
                return "";
            condition = __Visitor.RouteExpressionHandler(expressionBody, firstLevel: true).SqlOut;
            //GetPrefix(typeof(TInner));
            return condition;
        }
        internal void AddInnerRelationCondition(TypeQuery inner, string condition)
        {
            __Relations[inner] += "  and " + condition;
        }
        internal void AddInnerRelation(TypeQuery typeQuery, JoinType joinType, string condition)
        {
            var inner = typeQuery.OriginType;
            if (__Relations.ContainsKey(typeQuery))
            {
                throw new CRLException(string.Format("关联查询已包含关联对象 {0} {1}", inner,condition));
                return;
            }
            //if (__MainType == inner)
            //{
            //    throw new CRLException(string.Format("关联查询不能指定自已 {0} {1}" , inner,condition));
            //    return;
            //}
            if (inner.IsSubclassOf(typeof(IModel)))
            {
                DBExtendFactory.CreateDBExtend(__DbContext).CheckTableCreated(inner);
            }
            var tableName = TypeCache.GetTableName(inner, __DbContext);

            string aliasName = GetPrefix(inner);
            aliasName = aliasName.Substring(0, aliasName.Length - 1);
            if (typeQuery.TypeQueryEnum == TypeQueryEnum.查询)
            {
                //查询别名按关联别名算
                condition = condition.Replace(typeQuery.queryName2, aliasName+".");
                tableName = string.Format("({0}) {1} ", typeQuery.InnerQuery, aliasName);
            }
            else
            {
                tableName = string.Format("{0} {1} {2}", __DBAdapter.KeyWordFormat(tableName), aliasName, __DBAdapter.GetWithNolockFormat(__WithNoLock));
            }
            string str = string.Format(" {0} join {1} on {2}", joinType, tableName, condition);
            if (!__Relations.ContainsKey(typeQuery))
            {
                __Relations.Add(typeQuery, str);
            }
        }
        #region Union
        internal class UnionQuery
        {
            public LambdaQueryBase query;
            public UnionType unionType;
        }
        internal List<UnionQuery> __Unions = new List<UnionQuery>();
        /// <summary>
        /// 在分表情况下,联合查询所有表方式
        /// </summary>
        internal UnionType __ShanrdingUnionType;
        internal void AddUnion(LambdaQueryBase query2, UnionType unionType)
        {
            if (unionType == UnionType.None)
            {
                throw new CRLException("没有指定UnionType");
            }
            __Unions.Add(new UnionQuery() { query = query2, unionType = unionType });
            foreach (var kv in query2.QueryParames)
            {
                QueryParames[kv.Key] = kv.Value;
            }
        }
        #endregion

        /// <summary>
        /// 获取查询字段字符串,按条件排除
        /// </summary>
        /// <returns></returns>
        internal abstract string GetQueryFieldString();
        //internal virtual string GetQueryFieldStringBase()
        //{
        //    return GetQueryFieldString();
        //}
        /// <summary>
        /// 获取查询条件串,带表名
        /// </summary>
        /// <returns></returns>
        internal abstract string GetQueryConditions(bool withTableName = true);
        /// <summary>
        /// 获取完整查询
        /// </summary>
        /// <returns></returns>
        internal abstract string GetQuery();
        SelectFieldInfo GetSelectFieldInfo()
        {
            if (_CurrentSelectFieldCache == null)
            {
                SelectAll();
            }
            return _CurrentSelectFieldCache;
        }
        internal IEnumerable<Attribute.FieldMapping> GetFieldMapping()
        {
            var cache = GetSelectFieldInfo();
            return cache.mapping;
        }
    }
}
