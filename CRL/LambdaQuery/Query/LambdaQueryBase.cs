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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CoreHelper;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.RegularExpressions;
namespace CRL.LambdaQuery
{
    public abstract partial class LambdaQueryBase
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
        internal List<Tuple<string, object>> QueryParames
        {
            get
            {
                return __Visitor.QueryParames;
            }
        }
        /// <summary>
        /// 条件
        /// </summary>
        internal StringBuilder Condition = new StringBuilder();
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
        /// group字段 QueryField
        /// </summary>
        internal List<Attribute.FieldMapping> __GroupFields;
        internal Dictionary<TypeQuery, JoinInfo> __Relations;
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
        //List<string> __queryOrderBy;
        //internal List<string> __QueryOrderBy
        //{
        //    get
        //    {
        //        __queryOrderBy = __queryOrderBy ?? new List<string>();
        //        return __queryOrderBy;
        //    }
        //    set
        //    {
        //        __queryOrderBy = __queryOrderBy ?? new List<string>();
        //        __queryOrderBy = value;
        //    }
        //}
        /// <summary>
        /// 填充参数
        /// </summary>
        /// <param name="db"></param>
        internal void FillParames(AbsDBExtend db)
        {
            //db.ClearParams();
            foreach (var n in QueryParames)
            {
                db.SetParam(n.Item1, n.Item2);
            }
        }
        internal bool __WithNoLock = true;
        //internal int __AutoInJoin = 0;
        //internal bool __QueryAllField = false;
        

        #region 别名
        /// <summary>
        /// 别名
        /// </summary>
        internal Dictionary<Type, string> __Prefixs = new Dictionary<Type, string>();
        //internal string __PrefixsAllKey;
        internal int prefixIndex = 0;
        static Dictionary<int, string> prefixDic = new Dictionary<int, string>() { { 1, "t1." }, { 2, "t2." }, { 3, "t3." }, { 4, "t4." }, { 5, "t5." }, { 6, "t6." }, { 7, "t7." }, { 8, "t8." }, { 9, "t9." }, { 10, "t10." }, { 11, "t11." }, { 12, "t12." } };
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
                string prefix2 = prefixDic[prefixIndex];
                if (!__UseTableAliasesName)
                {
                    prefix2 = "";
                }
                __Prefixs[type] = prefix2;
                //__PrefixsAllKey = string.Join("-", __Prefixs.Keys);
                return prefix2;
            }
            return prefix;
        }
        #endregion
        internal static System.Collections.Concurrent.ConcurrentDictionary <string, SelectFieldInfo> queryFieldCache = new System.Collections.Concurrent.ConcurrentDictionary<string, SelectFieldInfo>();
        internal void SelectAll(bool cacheAllFieldString = true)
        {
            var cache = false;
            string key = "";
            //当选择所有字段时,进行缓存
            if (GetPrefix(__MainType) == "t1.")
            {
                key = __MainType.ToString();
                SelectFieldInfo value;
                var a = queryFieldCache.TryGetValue(key, out value);
                if (a)
                {
                    if (!cacheAllFieldString)
                    {
                        var item = value.Clone();
                        item.CleanQueryFieldString();
                        _CurrentSelectFieldCache = item;
                    }
                    else
                    {
                        _CurrentSelectFieldCache = value;
                    }
                    return;
                }
                cache = true;
            }
            var exp = Expression.Parameter(__MainType, "b");
            var fields = GetSelectField(true, exp, false, __MainType);
            if(cache)
            {
                var clone = fields.Clone();
                clone.GetQueryFieldString();
                queryFieldCache.TryAdd(key, clone);
            }
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
        //internal void AddInnerRelationCondition(TypeQuery inner, string condition)
        //{
        //    __Relations[inner] += "  and " + condition;
        //}
        internal void AddInnerRelation(TypeQuery typeQuery, JoinType joinType, string condition)
        {
            if (__Relations == null)
            {
                __Relations = new Dictionary<TypeQuery, JoinInfo>();
            }
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
            //string str = string.Format(" {0} join {1} on {2}", joinType, tableName, condition);
            if (!__Relations.ContainsKey(typeQuery))
            {
                var join = new JoinInfo() { joinType = joinType, tableName = tableName, condition = condition };
                __Relations.Add(typeQuery, join);
            }
        }
        #region Union
        internal class UnionQuery
        {
            public LambdaQueryBase query;
            public UnionType unionType;
        }
        internal List<UnionQuery> __Unions;
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
            __Unions = __Unions ?? new List<UnionQuery>();
            __Unions.Add(new UnionQuery() { query = query2, unionType = unionType });
            QueryParames.AddRange(query2.QueryParames);
            //foreach (var kv in query2.QueryParames)
            //{
            //    QueryParames[kv.Item1] = kv.Item2;
            //}
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
        internal virtual void GetQueryConditions(StringBuilder sb, bool withTableName = true)
        {
            //return "";
        }
        /// <summary>
        /// 获取完整查询
        /// </summary>
        /// <returns></returns>
        internal abstract string GetQuery();
        internal SelectFieldInfo GetSelectFieldInfo()
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
        #region In关联优化
        internal string __RemoveInJionBatchNo;
        #endregion

        #region 排序
        internal void SetOrder(Attribute.FieldMapping field, bool desc)
        {
            var str = field.QueryField + (desc ? " desc" : " asc");
            if (__QueryOrderBy != "")
            {
                str = "," + str;
            }
            __QueryOrderBy += str;
        }
        internal string GetOrder()
        {
            return __QueryOrderBy;
        }
        internal void CleanOrder()
        {
            __QueryOrderBy = "";
        }
        #endregion
    }

    [Attribute.Table(TableName = "__InJoin")]
    class InJoin : CRL.IModelBase
    {
        public void SetValue(Type type,object v)
        {
            switch (type.Name)
            {
                case "Int32":
                    this.V_Int32 = (int)v;
                    break;
                case "Decimal":
                    this.V_Decimal = (decimal)v;
                    break;
                case "String":
                    this.V_String = v.ToString();
                    break;
                case "Double":
                    this.V_Double = (double)v;
                    break;
            }
        }
        [CRL.Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集,Length =50)]
        public string BatchNo
        {
            get; set;
        }
        public int V_Int32
        {
            get; set;
        }
        public decimal V_Decimal
        {
            get; set;
        }
        [CRL.Attribute.Field(Length =50)]
        public string V_String
        {
            get; set;
        }
        public double V_Double
        {
            get; set;
        }
    }

    internal class JoinInfo
    {
        public JoinType joinType;
        public string tableName;
        public string condition;
        public override string ToString()
        {
            return string.Format(" {0} join {1} on {2}", joinType, tableName, condition);
        }
    }
}
