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
        internal ParameCollection QueryParames
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
        /// <summary>
        /// 查询的字段
        /// </summary>
        internal List<Attribute.FieldAttribute> __QueryFields = new List<CRL.Attribute.FieldAttribute>();
        /// <summary>
        /// group字段
        /// </summary>
        internal List<Attribute.FieldAttribute> __GroupFields = new List<CRL.Attribute.FieldAttribute>();
        internal Dictionary<TypeQuery, string> __Relations = new Dictionary<TypeQuery, string>();
        internal DbContext __DbContext;
        internal DBAdapter.DBAdapterBase __DBAdapter;
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
        #region 解析选择的字段
        /// <summary>
        /// 解析选择的字段
        /// </summary>
        /// <param name="isSelect">查询字段时按属性名生成别名</param>
        /// <param name="expressionBody"></param>
        /// <param name="withTablePrefix">是否生按表生成前辍,关联时用 如Table__Name</param>
        /// <param name="types"></param>
        /// <returns></returns>
        internal List<Attribute.FieldAttribute> GetSelectField(bool isSelect,Expression expressionBody, bool withTablePrefix, params Type[] types)
        {
            var allFilds = new Dictionary<Type, IgnoreCaseDictionary<Attribute.FieldAttribute>>();
            allFilds.Add(__MainType, TypeCache.GetProperties(__MainType, true));
            foreach (var t in types)
            {
                if (!allFilds.ContainsKey(t))
                {
                    allFilds.Add(t, TypeCache.GetProperties(t, true));
                }
            }
            List<Attribute.FieldAttribute> resultFields = new List<Attribute.FieldAttribute>();

            if (expressionBody is NewExpression)//按匿名对象
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
                        var methodQuery = getSelectMethodCall(methodCallExpression, out methodMember);
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
                        if (memberExpression.Expression.Type.BaseType == typeof(object))
                        {
                            //按匿名对象属性,视图关联时用
                            var f2 = new Attribute.FieldAttribute() { MemberName = memberExpression.Member.Name };
                            f2.SetFieldQueryScript2(__DBAdapter, GetPrefix(memberExpression.Expression.Type), withTablePrefix, memberName);
                            resultFields.Add(f2);
                            continue;

                        }
                        if (!allFilds[memberExpression.Expression.Type].ContainsKey(memberExpression.Member.Name))
                        {
                            throw new CRLException("找不到可筛选的属性" + memberExpression.Member.Name + " 在" + memberExpression.Expression.Type);
                        }
                        var f = allFilds[memberExpression.Expression.Type][memberExpression.Member.Name].Clone();
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
            }
            else if (expressionBody is MethodCallExpression)
            {
                #region 方法
                var method = expressionBody as MethodCallExpression;
                var f = new Attribute.FieldAttribute() { ModelType = __MainType };
                string methodMember;
                var methodQuery = getSelectMethodCall(expressionBody, out methodMember);
                f.SetFieldQueryScript2(__DBAdapter, "", withTablePrefix, "", methodQuery);
                f.FieldQuery = new Attribute.FieldQuery() { MemberName = methodMember, FieldName = methodMember, MethodName = method.Method.Name };
                resultFields.Add(f);
                #endregion
            }
            else if (expressionBody is BinaryExpression)
            {
                var field = getSeletctBinary(expressionBody);
                var f = new Attribute.FieldAttribute() { ModelType = __MainType };
                f.SetFieldQueryScript2(__DBAdapter, "", withTablePrefix, "", field);
                f.FieldQuery = new Attribute.FieldQuery() { MemberName = f.MemberName, FieldName = field, MethodName = "" };
                resultFields.Add(f);
            }
            else if (expressionBody is ConstantExpression)
            {
                var constant = (ConstantExpression)expressionBody;
                var f = new Attribute.FieldAttribute() { ModelType = __MainType };
                f.SetFieldQueryScript2(__DBAdapter, "", withTablePrefix, "", constant.Value + "");
                f.FieldQuery = new Attribute.FieldQuery() { MemberName = f.MemberName, FieldName = constant.Value + "", MethodName = "" };
                resultFields.Add(f);
            }
            else if (expressionBody is UnaryExpression)
            {
                var unaryExpression = expressionBody as UnaryExpression;
                return GetSelectField(false,unaryExpression.Operand, withTablePrefix, types);
            }
            else if (expressionBody is MemberExpression)//按成员
            {
                var mExp = (MemberExpression)expressionBody;
                if (mExp.Expression.Type.BaseType == typeof(object))
                {
                    //按匿名对象属性,视图关联时用
                    var f2 = new Attribute.FieldAttribute() { MemberName = mExp.Member.Name };
                    f2.SetFieldQueryScript2(__DBAdapter, GetPrefix(mExp.Expression.Type), withTablePrefix, mExp.Member.Name);
                    resultFields.Add(f2);
                    return resultFields; ;

                }

                if (!allFilds[mExp.Expression.Type].ContainsKey(mExp.Member.Name))
                {
                    throw new CRLException("找不到可筛选的属性" + mExp.Member.Name + " 在" + mExp.Expression.Type);
                }
                var f = allFilds[mExp.Expression.Type][mExp.Member.Name].Clone();
                f.SetFieldQueryScript2(__DBAdapter, GetPrefix(f.ModelType), withTablePrefix, "");
                f.FieldQuery = new Attribute.FieldQuery() { MemberName = f.MemberName, FieldName = f.MapingName, MethodName = "" };
                resultFields.Add(f);
            }
            else
            {
                throw new CRLException("不支持此语法解析:" + expressionBody);
            }
            return resultFields;
        }
        /// <summary>
        /// 返回方法调用拼接
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        string getSelectMethodCall(Expression expression, out string memberName)
        {
            var method = expression as MethodCallExpression;
            MemberExpression memberExpression;
            var args = method.Arguments[0];
            memberName = "";
            string methodArgs = "";
            if (args is ParameterExpression)
            {
                var exp2 = method.Arguments[1] as UnaryExpression;
                var type = exp2.Operand.GetType();
                var p = type.GetProperty("Body");
                var exp3 = p.GetValue(exp2.Operand, null) as Expression;
                methodArgs = getSeletctBinary(exp3);
                memberName = "";
            }
            else if (args is UnaryExpression)//like a.Code.Count()
            {
                memberExpression = (args as UnaryExpression).Operand as MemberExpression;
                memberName = memberExpression.Member.Name;
                methodArgs = GetPrefix(memberExpression.Expression.Type) + memberExpression.Member.Name;
            }
            else if (args is MemberExpression)
            {
                //like a.Code
                memberExpression = args as MemberExpression;
                memberName = memberExpression.Member.Name;
                methodArgs = GetPrefix(memberExpression.Expression.Type) + memberExpression.Member.Name;
            }
            else
            {
                throw new CRLException("不支持此语法解析:" + args);
            }
            string methodName = method.Method.Name;

            return string.Format("{0}({1})", methodName, methodArgs);
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
                return prefix2;
            }
            return prefix;
        }
        #endregion
        internal void SelectAll()
        {
            var all = TypeCache.GetTable(__MainType).Fields;
            __QueryFields.Clear();
            foreach (var item in all)
            {
                var item2 = item.Clone();
                item2.SetFieldQueryScript2(__DBAdapter, GetPrefix(item2.ModelType), false, "");
                __QueryFields.Add(item2);
            }
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
            var result = __Visitor.RouteExpressionHandler(expressionBody,firstLevel:true);
            if (string.IsNullOrEmpty(result.SqlOut))//没有构成树
            {
                string typeStr2 = "";
                result.SqlOut = __Visitor.DealParame(result, "", out typeStr2).Data + "";
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
                tableName = string.Format("{0} {1} {2}", __DBAdapter.KeyWordFormat(tableName), aliasName, __DBAdapter.GetWithNolockFormat());
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
        internal List<Attribute.FieldMapping> GetFieldMapping()
        {
            if (__QueryFields.Count == 0)
            {
                SelectAll();
            }
            return __QueryFields.Select(b => b.FieldMapping).ToList();
        }
    }
}
