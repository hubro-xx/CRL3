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
    public abstract partial class LambdaQuery<T> : LambdaQueryBase where T : IModel, new()
    {
        /// <summary>
        /// lambda查询
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="_useTableAliasesName">查询是否生成表别名,在更新和删除时用</param>
        public LambdaQuery(DbContext _dbContext, bool _useTableAliasesName = true)
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
            //string fields = GetQueryFieldString();
            //return string.Format("{0}{1}{2}{3}", __QueryTop, fields, QueryTableName, Condition);
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
        internal void FillParames(AbsDBExtend db)
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
        internal string QueryTableName = "";
        /// <summary>
        /// 条件
        /// </summary>
        protected string Condition = "";
        ///// <summary>
        ///// 前几条
        ///// </summary>
        //internal int __QueryTop = 0;
        /// <summary>
        /// 使用函数格式化字段
        /// </summary>
        internal string __FieldFunctionFormat = "";

        /// <summary>
        /// 获取记录条数
        /// </summary>
        public int TakeNum = 0;
        /// <summary>
        /// 分页索引,要分页,设为大于1
        /// </summary>
        public int SkipPage = 0;

        /// <summary>
        /// group having
        /// </summary>
        protected string Having = "";
        /// <summary>
        /// 是否编译为存储过程
        /// </summary>
        internal bool __CompileSp;

        #endregion


        protected DateTime startTime;
       
        #region 对外方法
        /// <summary>
        /// 设置查询TOP
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public LambdaQuery<T> Top(int top)
        {
            TakeNum = top;
            return this;
        }
        /// <summary>
        /// 设置查询TOP
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public LambdaQuery<T> Take(int top)
        {
            TakeNum = top;
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
            TakeNum = pageSize;
            SkipPage = pageIndex;
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
            var fields = GetSelectField(true,resultSelectorBody, false, typeof(T));
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
        public abstract LambdaQuery<T> Where(Expression<Func<T, bool>> expression);
        #endregion

        #region order
        /// <summary>
        /// 设置排序 可累加
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="desc">是否倒序</param>
        /// <returns></returns>
        public abstract LambdaQuery<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression, bool desc = true);
       
        /// <summary>
        /// 按主键排序
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        public abstract LambdaQuery<T> OrderByPrimaryKey(bool desc);
        #endregion

        #region OR
        /// <summary>
        /// 按当前条件累加OR条件
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public abstract LambdaQuery<T> Or(Expression<Func<T, bool>> expression);
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

        protected abstract LambdaQuery<T> InnerSelect<TInner>(Expression<Func<T, object>> outField, Expression<Func<TInner, object>> innerField,
     Expression<Func<T, TInner, bool>> expression, string type) where TInner : IModel, new();
        #endregion
        #endregion

        #region 获取解析值
        

        /// <summary>
        /// 获取查询字段字符串,按条件排除
        /// </summary>
        /// <param name="removes"></param>
        /// <returns></returns>
        internal abstract string GetQueryFieldString(Predicate<Attribute.FieldAttribute> removes = null);

        /// <summary>
        /// 是否为关联更新/删除
        /// </summary>
        internal bool _IsRelationUpdate = false;
        /// <summary>
        /// 获取查询条件串,带表名
        /// </summary>
        /// <returns></returns>
        internal abstract string GetQueryConditions(bool withTableName = true);
        /// <summary>
        /// 获取排序 带 order by
        /// </summary>
        /// <returns></returns>
        internal abstract string GetOrderBy();

        /// <summary>
        /// 获取完整查询
        /// </summary>
        /// <returns></returns>
        internal abstract string GetQuery();
        /// <summary>
        /// 输出当前查询语句
        /// </summary>
        /// <param name="uselog">是否生成到文件</param>
        /// <returns></returns>
        public string PrintQuery(bool uselog = false)
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
