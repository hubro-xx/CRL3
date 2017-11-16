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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CRL.LambdaQuery
{
    public sealed partial class RelationLambdaQuery<T> : LambdaQuery<T> where T : IModel, new()
    {
        /// <summary>
        /// lambda查询
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="_useTableAliasesName">查询是否生成表别名,在更新和删除时用</param>
        public RelationLambdaQuery(DbContext _dbContext, bool _useTableAliasesName = true)
            : base(_dbContext, _useTableAliasesName)
        {
           
        }

        /// <summary>
        /// 设置条件 可累加，按and
        /// </summary>
        /// <param name="expression">最好用变量代替属性或方法</param>
        /// <returns></returns>
        public override LambdaQuery<T> Where(Expression<Func<T, bool>> expression)
        {
            if (expression == null)
                return this;
            //var b = expression.ToString();
            //if (QueryFields.Count == 0)
            //{
            //    SelectAll();
            //}
            string condition = FormatExpression(expression.Body).SqlOut;
            if (Condition.Length > 0)
            {
                condition = " and " + condition;
            }
            Condition.Append(condition);
            expression = null;
            //this.Condition += string.IsNullOrEmpty(Condition) ? condition : " and " + condition;
            return this;
        }
        /// <summary>
        /// 设置排序 可累加
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="desc">是否倒序</param>
        /// <returns></returns>
        public override LambdaQuery<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression, bool desc = true)
        {
            var parameters = expression.Parameters.Select(b => b.Type).ToArray();
            var fields = GetSelectField(false, expression.Body, false, parameters).mapping;
            SetOrder(fields.First(), desc);
            //if (!string.IsNullOrEmpty(__QueryOrderBy))
            //{
            //    __QueryOrderBy += ",";
            //}
            //__QueryOrderBy += string.Format(" {0} {1}", fields.First().QueryField, desc ? "desc" : "asc");
            return this;
        }
        /// <summary>
        /// 按主键排序
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        public override LambdaQuery<T> OrderByPrimaryKey(bool desc)
        {
            var key = TypeCache.GetTable(typeof(T)).PrimaryKey;
            if (key == null)
            {
                return this;
            }
            var field = new Attribute.FieldMapping() { QueryField = GetPrefix() + key.MapingName, PropertyType = key.PropertyType };
            SetOrder(field, desc);
            //if (!string.IsNullOrEmpty(__QueryOrderBy))
            //{
            //    __QueryOrderBy += ",";
            //}
            //var key = TypeCache.GetTable(typeof(T)).PrimaryKey;
            //if (key == null)
            //{
            //    return this;
            //}
            //__QueryOrderBy += string.Format(" {2}{0} {1}", key.MapingName, desc ? "desc" : "asc", GetPrefix());
            //QueryOrderBy = ReplacePrefix(QueryOrderBy);
            return this;
        }
        /// <summary>
        /// 按当前条件累加OR条件
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override LambdaQuery<T> Or(Expression<Func<T, bool>> expression)
        {
            string condition1 = FormatExpression(expression.Body).SqlOut;
            //this.Condition = string.Format("({0}) or {1}", Condition, condition1);
            Condition.AppendFormat(" or {0}", condition1);
            return this;
        }
        LambdaQuery<T> __InnerSelect<TInner>(Expression<Func<T, object>> outField, Expression<Func<TInner, object>> innerField,
    Expression<Func<T, TInner, bool>> expression, string type)
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
            condition = string.Format("{0} {1}(select {2} from {3} where {4})", field1, type, field2, tableName + __DBAdapter.GetWithNolockFormat(__WithNoLock), condition);
            //this.Condition += string.IsNullOrEmpty(Condition) ? condition : " and " + condition;
            if (Condition.Length > 0)
            {
                condition = " and " + condition;
            }
            Condition.Append(condition);
            return this;
        }
        internal override string GetQueryFieldString()
        {
            GetSelectFieldInfo();
            if (_CurrentSelectFieldCache.mapping.Count() == 0)
            {
                throw new CRLException("选择的列不能为空");
            }
            return _CurrentSelectFieldCache.GetQueryFieldString();
        }

        /// <summary>
        /// 获取查询条件串,带表名
        /// </summary>
        /// <returns></returns>
        internal override void GetQueryConditions(StringBuilder part, bool withTableName = true)
        {
            
            string where = Condition.ToString();
            //var part = new StringBuilder();
            if (withTableName)
            {
                //part.Append(" from ");
                var prex1 = GetPrefix(__MainType);
                prex1 = prex1.Substring(0, prex1.Length - 1);
                part.AppendFormat(" from {0} {1} {2}", __DBAdapter.KeyWordFormat(QueryTableName), prex1, __DBAdapter.GetWithNolockFormat(__WithNoLock));
            }
            string join = "";
            if (__Relations != null)
            {
                join = string.Join(" ", __Relations.Values);
            }
            part.AppendFormat(" {0}{1}", join, where.Length == 0 ? " " : " where " + where);
            #region group判断
            if (__GroupFields!=null)
            {
                part.Append(" group by ");
                part.Append(string.Join(",", __GroupFields.Select(b => b.QueryField)));
            }
            if (!string.IsNullOrEmpty(Having))
            {
                part.Append(" having " + Having);
            }
            #endregion

            //return part.ToString();
        }
        /// <summary>
        /// 获取排序 带 order by
        /// </summary>
        /// <returns></returns>
        internal override string GetOrderBy()
        {
            string orderBy = "";
            if (GetOrder() == "")
            {
                orderBy = TypeCache.GetTable(typeof(T)).DefaultSort;
            }
            else
            {
                orderBy = GetOrder();
            }
            orderBy = string.IsNullOrEmpty(orderBy) ? orderBy : " order by " + orderBy;
            return orderBy;
        }
        /// <summary>
        /// 获取完整查询
        /// </summary>
        /// <returns></returns>
        internal override string GetQuery()
        {
            return GetQueryOrigin();
        }

        string GetQueryOrigin()
        {
            string fields = GetQueryFieldString();
            if (!string.IsNullOrEmpty(__FieldFunctionFormat))
            {
                fields = string.Format(__FieldFunctionFormat, fields);
            }
            if (distinctCount)
            {
                fields = Regex.Replace(fields, @" as \w+", "");//替换别名
                fields = string.Format(" count({0}) as Total", fields);
                if (_CurrentSelectFieldCache.mapping.Count > 1)
                {
                    throw new CRLException("distinct 时,不能count多个字段 " + fields);
                }
            }

            //var part = " from " + GetQueryConditions();

            var orderBy = GetOrderBy();
            var sql = new StringBuilder();
            //当设置了分表联合查询
            if (__DbContext.UseSharding && __ShanrdingUnionType != UnionType.None)
            {
                #region 当设置了分表联合查询
                string tableName = TypeCache.GetTable(typeof(T)).TableName;
                var tables = Sharding.DBService.GetAllTable(__DbContext.DBLocation.ShardingDataBase, tableName);
                string unionType = __ShanrdingUnionType == UnionType.Union ? "union" : "union all";
                //var dbExtend = new DBExtend(dbContext); //todo 检查分表是否被创建
                for (int i = 0; i < tables.Count; i++)
                {
                    var table = tables[i];
                    //var part1 = part.Replace("from " + __DBAdapter.KeyWordFormat(tableName), "from " + __DBAdapter.KeyWordFormat(table.PartName));
                    //sql.Append(__DBAdapter.GetSelectTop(fields, part1, "", TakeNum));
                    __DBAdapter.GetSelectTop(sql, fields, b =>
                    {
                        GetQueryConditions(b);
                        b.Replace("from " + __DBAdapter.KeyWordFormat(tableName), "from " + __DBAdapter.KeyWordFormat(table.PartName));
                    }, "", TakeNum);
                    if (i < tables.Count - 1)
                    {
                        sql.Append("\r\n " + unionType + " \r\n");
                    }
                }
                orderBy = Regex.Replace(orderBy, @"t\d+\.", "");
                sql.Append("\r\n " + orderBy);
                #endregion
            }
            else
            {

                if (__Unions == null)//默认
                {
                    //var sql2 = __DBAdapter.GetSelectTop(fields, part, orderBy, TakeNum);
                    //sql.Append(sql2);
                    __DBAdapter.GetSelectTop(sql, fields, b =>
                     {
                         GetQueryConditions(b);
                     }, orderBy, TakeNum);
                }
                else
                {
                    #region 联合查询
                    //var sql2 = __DBAdapter.GetSelectTop(fields, part, "", TakeNum);
                    //sql.Append(sql2);
                    __DBAdapter.GetSelectTop(sql, fields, b =>
                     {
                         GetQueryConditions(b);
                     }, "", TakeNum);
                    foreach (var unionQuery in __Unions)
                    {
                        var query = unionQuery.query;
                        query.CleanOrder();
                        string unionType = unionQuery.unionType == UnionType.Union ? "union" : "union all";
                        var sqlUnoin = query.GetQuery();
                        sql.Append("\r\n " + unionType + " \r\n");
                        sql.Append(sqlUnoin);
                    }
                    if (!string.IsNullOrEmpty(orderBy))
                    {
                        orderBy = System.Text.RegularExpressions.Regex.Replace(orderBy, @"t\d+\.", " ");
                        sql.Append("\r\n " + orderBy);
                    }
                    #endregion
                }
            }
            var ts = DateTime.Now - startTime;
            AnalyticalTime = ts.TotalMilliseconds;
            return sql.ToString();
        }
    }
}
