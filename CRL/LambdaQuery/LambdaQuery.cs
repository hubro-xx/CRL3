/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using CoreHelper;
//Lambda 表达式参考
//http://msdn.microsoft.com/zh-cn/library/bb397687.aspx
//http://www.cnblogs.com/hubro/p/4381337.html
namespace CRL.LambdaQuery
{
    /// <summary>
    /// Lamada表达式查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed partial class LambdaQuery<T> : LambdaQueryBase where T : IModel, new()
    {
        /// <summary>
        /// lambda查询
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="_useTableAliasesName">查询是否生成表别名,在更新和删除时用</param>
        internal LambdaQuery(DbContext _dbContext, bool _useTableAliasesName = true)
        {
            __DbContext = _dbContext;
            __MainType = typeof(T);
            __DBAdapter = DBAdapter.DBAdapterBase.GetDBAdapterBase(_dbContext);
            __UseTableAliasesName = _useTableAliasesName;
            __Visitor = new ExpressionVisitor(__DBAdapter);
            //TypeCache.SetDBAdapterCache(typeof(T), dBAdapter);
            GetPrefix(typeof(T));
            QueryTableName = TypeCache.GetTableName(typeof(T), __DbContext);
            startTime = DateTime.Now;
        }
        
        /// <summary>
        /// 返回查询唯一值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetQuery();
            string fields = GetQueryFieldString();
            return string.Format("{0}{1}{2}{3}", __QueryTop, fields, QueryTableName, Condition);
        }
        #region 字段
        /// <summary>
        /// 在分表情况下,联合查询所有表方式
        /// </summary>
        internal Sharding.UnionType __UnionType;
        /// <summary>
        /// 查询返回的总行数
        /// </summary>
        public int RowCount = 0;
        /// <summary>
        /// 缓存查询过期时间
        /// </summary>
        internal int __ExpireMinute = 0;
        /// <summary>
        /// 处理后的查询参数
        /// </summary>
        ParameCollection QueryParames
        {
            get
            {
                return __Visitor.QueryParames;
            }
        }
        /// <summary>
        /// 语法解析时间
        /// </summary>
        public double AnalyticalTime = 0;
        /// <summary>
        /// 语句执行时间
        /// </summary>
        public double ExecuteTime;
        /// <summary>
        /// 对象转换时间
        /// </summary>
        public double MapingTime=0;
        /// <summary>
        /// 填充参数
        /// </summary>
        /// <param name="db"></param>
        internal void FillParames(DBExtend db)
        {
            //db.ClearParams();
            foreach (var n in QueryParames)
            {
                db.SetParam(n.Key, n.Value);
            }
        }

        internal List<Attribute.FieldAttribute> GetQueryFields()
        {
            if (__QueryFields.Count == 0)
            {
                SelectAll();
            }
            return __QueryFields;
        }

        /// <summary>
        /// 查询的表名
        /// </summary>
        string QueryTableName = "";
        /// <summary>
        /// 条件
        /// </summary>
        string Condition = "";
        /// <summary>
        /// 前几条
        /// </summary>
        internal int __QueryTop = 0;
        /// <summary>
        /// 使用函数格式化字段
        /// </summary>
        internal string __FieldFunctionFormat = "";

        /// <summary>
        /// 分页每页大小
        /// </summary>
        public int PageSize = 0;
        /// <summary>
        /// 分页索引
        /// </summary>
        public int PageIndex = 0;

        /// <summary>
        /// group having
        /// </summary>
        string Having = "";
        /// <summary>
        /// 是否编译为存储过程
        /// </summary>
        internal bool __CompileSp;

        #endregion

        
        DateTime startTime;
       
        #region 对外方法
        /// <summary>
        /// 设置查询TOP
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public LambdaQuery<T> Top(int top)
        {
            __QueryTop = top;
            //return Select(top, null);
            return this;
        }
        /// <summary>
        /// 投置缓存查询过期时间
        /// </summary>
        /// <param name="expireMinute"></param>
        /// <returns></returns>
        public LambdaQuery<T> Expire(int expireMinute)
        {
            __ExpireMinute = expireMinute;
            return this;
        }
        /// <summary>
        /// 设定分页参数
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public LambdaQuery<T> Page(int pageSize = 15, int pageIndex = 1)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            __CompileSp = pageSize > 0;
            return this;
        }
        /// <summary>
        /// 设置是否编译为存储过程
        /// </summary>
        /// <param name="compileSp"></param>
        /// <returns></returns>
        public LambdaQuery<T> CompileToSp(bool compileSp)
        {
            __CompileSp = compileSp;
            return this;
        }
        /// <summary>
        /// 设置分表查询时,union方式
        /// </summary>
        /// <param name="unionType"></param>
        /// <returns></returns>
        public LambdaQuery<T> ShardingUnion(Sharding.UnionType unionType)
        {
            __UnionType = unionType;
            return this;
        }
        #region UnSelect

        /// <summary>
        /// 按条件排除字段
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public LambdaQuery<T> UnSelect(Predicate<Attribute.FieldAttribute> match)
        {
            var fields = TypeCache.GetProperties(typeof(T), false).Values.ToList();
            if (match != null)
            {
                fields.RemoveAll(match);
            }
            //string aliasName = GetPrefix();
            foreach(var item in fields)
            {
                //item.SetFieldQueryScript(aliasName, true, false);
                item.SetFieldQueryScript2(__DBAdapter, true, false, "");
            }
            __QueryFields = fields;
            return this;
        }
        #endregion

        #region Select

        /// <summary>
        /// 使用匿名类型选择查询字段
        /// </summary>
        /// <param name="resultSelector">like b=>new {b.Name}</param>
        /// <returns></returns>
        public LambdaQuery<T> Select(Expression<Func<T, object>> resultSelector)
        {
            if (resultSelector == null)
            {
                SelectAll();
                return this;
            }
            return Select(resultSelector.Body);
        }
        
        /// <summary>
        /// 按resultSelectorBody
        /// </summary>
        /// <param name="resultSelectorBody"></param>
        /// <returns></returns>
        internal LambdaQuery<T> Select(Expression resultSelectorBody)
        {
            var fields = GetSelectField(resultSelectorBody, false, typeof(T));
            __QueryFields = fields;
            return this;
        }
        #endregion

        #region where
        /// <summary>
        /// 设置条件 可累加，按and
        /// </summary>
        /// <param name="expression">最好用变量代替属性或方法</param>
        /// <returns></returns>
        public LambdaQuery<T> Where(Expression<Func<T, bool>> expression)
        {
            if (expression == null)
                return this;
            var b = expression.ToString();
            //if (QueryFields.Count == 0)
            //{
            //    SelectAll();
            //}
            string condition = FormatExpression(expression.Body);
            this.Condition += string.IsNullOrEmpty(Condition) ? condition : " and " + condition;
            return this;
        }
        /// <summary>
        /// 直接字符串查询
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        LambdaQuery<T> Where(string condition)
        {
            this.Condition += string.IsNullOrEmpty(Condition) ? condition : " and " + condition;
            return this;
        }
        #endregion

        #region order
        /// <summary>
        /// 设置排序 可累加
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="desc">是否倒序</param>
        /// <returns></returns>
        public LambdaQuery<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression, bool desc = true)
        {
            var fields = GetSelectField(expression.Body, false, typeof(T));
            if (!string.IsNullOrEmpty(__QueryOrderBy))
            {
                __QueryOrderBy += ",";
            }
            __QueryOrderBy += string.Format(" {0} {1}", fields.First().QueryField, desc ? "desc" : "asc");
            __QueryOrderBy = ReplacePrefix(__QueryOrderBy);
            return this;
        }
       
        /// <summary>
        /// 按主键排序
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        public LambdaQuery<T> OrderByPrimaryKey(bool desc)
        {
            if (!string.IsNullOrEmpty(__QueryOrderBy))
            {
                __QueryOrderBy += ",";
            }
            var key = TypeCache.GetTable(typeof(T)).PrimaryKey;
            __QueryOrderBy += string.Format(" {2}{0} {1}", key.Name, desc ? "desc" : "asc", GetPrefix());
            //QueryOrderBy = ReplacePrefix(QueryOrderBy);
            return this;
        }
        #endregion

        #region OR
        /// <summary>
        /// 按当前条件累加OR条件
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> Or(Expression<Func<T, bool>> expression)
        {
            string condition1 = FormatExpression(expression.Body);
            this.Condition = string.Format("({0}) or {1}", Condition, condition1);
            return this;
        }
        #endregion

        #region exists
        /// <summary>
        /// 按查询exists
        /// 等效为exixts(select field from table2)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="innerField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> Exists<TInner>(Expression<Func<TInner, object>> innerField,
Expression<Func<T, TInner, bool>> expression) where TInner : IModel, new()
        {
            return InnerSelect<TInner>(null, innerField, expression, "exists");
        }
        /// <summary>
        /// 按查询not exists
        /// 等效为not exixts(select field from table2)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="innerField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> NotExists<TInner>(Expression<Func<TInner, object>> innerField,
Expression<Func<T, TInner, bool>> expression) where TInner : IModel, new()
        {
            return InnerSelect<TInner>(null, innerField, expression, "not exists");
        }
        #endregion
        #region select值判断
        /// <summary>
        /// 按查询in
        /// 等效为table.field in(select field from table2)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="outField"></param>
        /// <param name="innerField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> In<TInner>(Expression<Func<T, object>> outField, Expression<Func<TInner, object>> innerField,
    Expression<Func<T, TInner, bool>> expression) where TInner : IModel, new()
        {
            return InnerSelect<TInner>(outField, innerField, expression, "in");
        }
        /// <summary>
        /// 按查询not in
        /// 等效为table.field not in(select field from table2)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="outField"></param>
        /// <param name="innerField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> NotIn<TInner>(Expression<Func<T, object>> outField, Expression<Func<TInner, object>> innerField,
Expression<Func<T, TInner, bool>> expression) where TInner : IModel, new()
        {
            return InnerSelect<TInner>(outField, innerField, expression, "not in");
        }
        /// <summary>
        /// 按=
        /// 等效为table.field =(select field from table2)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="outField"></param>
        /// <param name="innerField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> Equal<TInner>(Expression<Func<T, object>> outField, Expression<Func<TInner, object>> innerField,
Expression<Func<T, TInner, bool>> expression) where TInner : IModel, new()
        {
            return InnerSelect<TInner>(outField, innerField, expression, "=");
        }
        /// <summary>
        /// 按!=
        /// 等效为table.field !=(select field from table2)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="outField"></param>
        /// <param name="innerField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQuery<T> NotEqual<TInner>(Expression<Func<T, object>> outField, Expression<Func<TInner, object>> innerField,
Expression<Func<T, TInner, bool>> expression) where TInner : IModel, new()
        {
            return InnerSelect<TInner>(outField, innerField, expression, "!=");
        }

        LambdaQuery<T> InnerSelect<TInner>(Expression<Func<T, object>> outField, Expression<Func<TInner, object>> innerField,
    Expression<Func<T, TInner, bool>> expression, string type) where TInner : IModel, new()
        {
            MemberExpression m1 = null, m2;
            //object 会生成UnaryExpression表达式 Convert(b=>b.UserId)
            if (outField != null)//兼容exists 可能为空
            {
                if (outField.Body is UnaryExpression)
                {
                    m1 = (outField.Body as UnaryExpression).Operand as MemberExpression;
                }
                else
                {
                    m1 = outField.Body as MemberExpression;
                }
            }
            if (innerField.Body is UnaryExpression)
            {
                m2 = (innerField.Body as UnaryExpression).Operand as MemberExpression;
            }
            else
            {
                m2 = innerField.Body as MemberExpression;
            }
            string field1 = "";
            if (outField != null)
            {
                field1 = string.Format("{0}{1}", GetPrefix(), __DBAdapter.KeyWordFormat(m1.Member.Name));
            }
            string field2 = string.Format("{0}{1}", GetPrefix(typeof(TInner)), __DBAdapter.KeyWordFormat(m2.Member.Name));
            //var visitor2 = new ExpressionVisitor<TInner>(dbContext);
            //string condition = visitor2.RouteExpressionHandler(expression.Body);
            string condition = FormatJoinExpression(expression.Body);

            condition = string.Format(condition, GetPrefix(typeof(TInner)), "");
            var tableName = TypeCache.GetTableName(typeof(TInner), __DbContext);
            tableName = tableName + " " + GetPrefix(typeof(TInner));
            tableName = tableName.Substring(0, tableName.Length - 1);
            condition = string.Format("{0} {1}(select {2} from {3} where {4})", field1, type, field2, tableName + __DBAdapter.GetWithNolockFormat(), condition);
            this.Condition += string.IsNullOrEmpty(Condition) ? condition : " and " + condition;
            return this;
        }
        #endregion
        #endregion

        #region 获取解析值
        
        ///// <summary>
        ///// 转换为SQL条件，并提取参数
        ///// </summary>
        ///// <param name="expression"></param>
        ///// <returns></returns>
        //internal string FormatExpression(Expression<Func<T, bool>> expression)
        //{
        //    string condition;
        //    if (expression == null)
        //        return "";
        //    condition = __Visitor.RouteExpressionHandler(expression.Body);
        //    condition = ReplacePrefix(condition);
        //    return condition;
        //}

        /// <summary>
        /// 获取查询字段字符串,按条件排除
        /// </summary>
        /// <param name="removes"></param>
        /// <returns></returns>
        internal string GetQueryFieldString(Predicate<Attribute.FieldAttribute> removes = null)
        {
            if (__QueryFields.Count == 0)
            {
                SelectAll();
            }
            List<Attribute.FieldAttribute> queryFields = __QueryFields;
            if (removes != null)
            {
                queryFields.RemoveAll(removes);
            }
            //找出需要关联的字段
            List<Attribute.FieldAttribute> constraint = queryFields.FindAll(b => b.FieldType == Attribute.FieldType.关联字段 || b.FieldType == Attribute.FieldType.关联对象);
            //找出关联和对应的字段
            int tabIndex = 2;
            foreach (Attribute.FieldAttribute a in constraint)
            {
                #region 关联约束
                tabIndex += 1;
                if (a.FieldType == Attribute.FieldType.关联字段 && a.ConstraintType == null)//虚拟字段,没有设置关联类型
                {
                    throw new Exception(string.Format("需指定关联类型:{0}.{1}.Attribute.Field.ConstraintType", typeof(T), a.Name));
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
                if (a.FieldType == Attribute.FieldType.关联字段)//只是关联字段
                {
                    //var resultField = innerFields.Find(b => b.Name.ToUpper() == a.ConstraintResultField.ToUpper());
                    var resultField = innerFields[a.ConstraintResultField];
                    if (resultField == null)
                    {
                        throw new Exception(string.Format("在类型{0}找不到 ConstraintResultField {1}", innerType, a.ConstraintResultField));
                    }
                    AddInnerRelation(innerType, condition);
                    queryFields.Add(resultField);
                }
                else//关联对象
                {
                    AddInnerRelation(innerType, condition);
                    queryFields.AddRange(innerFields.Values);
                }
                #endregion
            }
            queryFields = queryFields.FindAll(b => b.FieldType == Attribute.FieldType.数据库字段 || b.FieldType == Attribute.FieldType.虚拟字段);
            string fields = Base.GetQueryFields(queryFields);
            fields = ReplacePrefix(fields);
            return fields;
        }
        /// <summary>
        /// 获取查询条件串,带表名
        /// </summary>
        /// <returns></returns>
        internal string GetQueryConditions()
        {
            string join = string.Join(" ", __Relations.Values);
            string where = Condition;
            where = string.IsNullOrEmpty(where) ? " 1=1 " : where;
            #region group判断
            if (__GroupFields.Count > 0)
            {
                where += " group by ";
                foreach (var item in __GroupFields)
                {
                    where += item.QueryField + ",";
                }
                where = where.Substring(0, where.Length - 1);
            }
            if (!string.IsNullOrEmpty(Having))
            {
                where += " having " + Having;
            }
            #endregion

            var part = string.Format("{0} t1 {1}  {2}  where {3}", __DBAdapter.KeyWordFormat(QueryTableName), __DBAdapter.GetWithNolockFormat(), join, where);
            part = ReplacePrefix(part);
            return part;
        }
        /// <summary>
        /// 获取排序 带 order by
        /// </summary>
        /// <returns></returns>
        internal string GetOrderBy()
        {
            string orderBy = __QueryOrderBy;
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = TypeCache.GetTable(typeof(T)).DefaultSort;
            }
            orderBy = string.IsNullOrEmpty(orderBy) ? orderBy : " order by " + orderBy;
            orderBy = ReplacePrefix(orderBy);
            return orderBy;
        }

        /// <summary>
        /// 获取完整查询
        /// </summary>
        /// <returns></returns>
        internal string GetQuery()
        {
            string fields = GetQueryFieldString();
            //if (DistinctFields)
            //{
            //    fields = string.Format(" distinct {0}", fields);
            //    if (distinctCount)
            //    {
            //        fields = string.Format(" count({0}) as Total", fields);
            //    }
            //}
            if (!string.IsNullOrEmpty(__FieldFunctionFormat))
            {
                fields = string.Format(__FieldFunctionFormat, fields);
            }
            if (distinctCount)
            {
                fields = string.Format(" count({0}) as Total", fields);
                if (__QueryFields.Count > 1)
                {
                    throw new Exception("distinct 时,不能count多个字段 " + fields);
                }
            }

            var part = " from " + GetQueryConditions();
            
            var orderBy = GetOrderBy();
            string sql = "";
            //当设置了分表关联
            if (__DbContext.UseSharding && __UnionType!= Sharding.UnionType.None)
            {
                string tableName = TypeCache.GetTable(typeof(T)).TableName;
                var tables = Sharding.DBService.GetAllTable(__DbContext.DBLocation.ShardingDataBase, tableName);
                string union = __UnionType == Sharding.UnionType.Union ? "union" : "union all";
                //var dbExtend = new DBExtend(dbContext); //todo 检查分表是否被创建
                for (int i = 0; i < tables.Count; i++)
                {
                    var table = tables[i];
                    var part1 = part.Replace("from " + __DBAdapter.KeyWordFormat(tableName), "from " + __DBAdapter.KeyWordFormat(table.PartName));
                    sql += __DBAdapter.GetSelectTop(fields, part1, "", __QueryTop);
                    if (i < tables.Count - 1)
                    {
                        sql += "\r\n" + union + "\r\n";
                    }
                }
                sql += orderBy;
            }
            else
            {
                sql = __DBAdapter.GetSelectTop(fields, part, orderBy, __QueryTop);
            }
            var ts = DateTime.Now - startTime;
            AnalyticalTime = ts.TotalMilliseconds;
            return sql;
        }
        /// <summary>
        /// 输出当前查询语句
        /// </summary>
        /// <param name="uselog">是否生成到文件</param>
        /// <returns></returns>
        public string PrintQuery(bool uselog=false)
        {
            string sql = GetQuery();
            string log = string.Format("[SQL]:{0}\r\n", sql);
            foreach (var item in QueryParames)
            {
                log += string.Format("[{0}]:[{1}]\r\n", item.Key, item.Value);
            }
            if (uselog)
            {
                CoreHelper.EventLog.Log(log, "LambdaQuery", false);
            }
            return log;
        }
        #endregion

        

    }
}
