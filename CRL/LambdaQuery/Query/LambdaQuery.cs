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
            GetPrefix(__MainType);
            __Visitor = new ExpressionVisitor(this);
            //TypeCache.SetDBAdapterCache(typeof(T), dBAdapter);
            QueryTableName = TypeCache.GetTableName(__MainType, __DbContext);
            startTime = DateTime.Now;
        }
        
        /// <summary>
        /// 返回查询语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetQuery();
            //string fields = GetQueryFieldString();
            //return string.Format("{0}{1}{2}{3}", __QueryTop, fields, QueryTableName, Condition);
        }
        #region 字段

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
        protected StringBuilder Condition = new StringBuilder();
        ///// <summary>
        ///// 前几条
        ///// </summary>
        //internal int __QueryTop = 0;
        /// <summary>
        /// 使用函数格式化字段
        /// </summary>
        internal string __FieldFunctionFormat = "";

        /// <summary>
        /// group having
        /// </summary>
        protected string Having = "";


        /// <summary>
        /// 是否自动跟踪对象状态
        /// </summary>
        internal bool __TrackingModel = true;
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
        ///设置当前查询是否跟踪对象状态
        /// </summary>
        /// <param name="trackingModel"></param>
        /// <returns></returns>
        public LambdaQuery<T> WithTrackingModel(bool trackingModel = true)
        {
            __TrackingModel = trackingModel;
            return this;
        }
        /// <summary>
        /// 设置分表查询时,union方式
        /// </summary>
        /// <param name="unionType"></param>
        /// <returns></returns>
        public LambdaQuery<T> ShardingUnion(UnionType unionType)
        {
            __ShanrdingUnionType = unionType;
            return this;
        }
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
        /// 返回强类型结果选择
        /// 兼容老写法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="resultSelector">为空则选择所有</param>
        /// <returns></returns>
        public LambdaQueryResultSelect<T, TResult> SelectV<TResult>(Expression<Func<T, TResult>> resultSelector = null)
        {
            //var fields = GetSelectField(true, resultSelector.Body, false, typeof(T));
            if (resultSelector == null)
            {
                SelectAll();
            }
            else
            {
                Select(resultSelector.Body);
            }
            return new LambdaQueryResultSelect<T, TResult>(this, resultSelector.Body);
        }
        /// <summary>
        /// 按resultSelectorBody
        /// </summary>
        /// <param name="resultSelectorBody"></param>
        /// <returns></returns>
        internal LambdaQuery<T> Select(Expression resultSelectorBody)
        {
            if (resultSelectorBody is ParameterExpression)
            {
                //按选择所有属性
                SelectAll();
                return this;
            }
            var fields = GetSelectField(true,resultSelectorBody, false, typeof(T));
            __QueryFields = fields;
            return this;
        }
        /// <summary>
        /// 创建一个相同上下文的Query
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <returns></returns>
        public LambdaQuery<T2> CreateQuery<T2>() where T2 : IModel, new()
        {
            var query = LambdaQueryFactory.CreateLambdaQuery<T2>(__DbContext);
            //重新排列前辍
            query.__Prefixs.Clear();
            query.__Prefixs[typeof(T)] = __Prefixs[typeof(T)];
            query.prefixIndex = base.prefixIndex;
            query.GetPrefix(typeof(T2));
            query.__FromDbContext = true;
            return query;
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

        #region select值判断

        /// <summary>
        /// 按查询exists
        /// 等效为exixts(select field from table2)
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public LambdaQuery<T> Exists<T2, TResult>(LambdaQueryResultSelect<T2, TResult> query)
        {
            return InnerSelect(null, query, "exists");
        }

        /// <summary>
        /// 按查询not exists
        /// 等效为 not exixts(select field from table2)
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public LambdaQuery<T> NotExists<T2, TResult>(LambdaQueryResultSelect<T2, TResult> query)
        {
            return InnerSelect(null, query, "not exists");
        }

        /// <summary>
        /// 按查询in
        /// 等效为table.field in(select field from table2)
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="outField"></param>
        /// <param name="expression">内关联</param>
        /// <returns></returns>
        public LambdaQuery<T> In<T2, TResult>(LambdaQueryResultSelect<T2, TResult> query, Expression<Func<T, TResult>> outField, Expression<Func<T, T2, bool>> expression = null)
        {
            return InnerSelect(outField, query, "in", expression);
        }

        /// <summary>
        /// 按查询not in
        /// 等效为table.field not in(select field from table2)
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="outField"></param>
        /// <returns></returns>
        public LambdaQuery<T> NotIn<T2, TResult>(LambdaQueryResultSelect<T2, TResult> query, Expression<Func<T, TResult>> outField)
        {
            return InnerSelect(outField, query, "not in");
        }

        /// <summary>
        /// 按=
        /// 等效为table.field =(select field from table2)
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="outField"></param>
        /// <returns></returns>
        public LambdaQuery<T> Equal<T2, TResult>(LambdaQueryResultSelect<T2, TResult> query, Expression<Func<T, TResult>> outField)
        {
            return InnerSelect(outField, query, "=");
        }
       
        /// <summary>
        /// 按!=
        /// 等效为table.field !=(select field from table2)
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="outField"></param>
        /// <returns></returns>
        public LambdaQuery<T> NotEqual<T2, TResult>(LambdaQueryResultSelect<T2, TResult> query, Expression<Func<T, TResult>> outField)
        {
            return InnerSelect(outField, query, "!=");
        }

        protected LambdaQuery<T> InnerSelect<T2, TResult>(Expression<Func<T, TResult>> outField, LambdaQueryResultSelect<T2, TResult> query, string type, Expression<Func<T, T2, bool>> expression = null)
        {
            if (!query.BaseQuery.__FromDbContext)
            {
                throw new CRLException("关联需要由LambdaQuery.CreateQuery创建");
            }
            MemberExpression m1 = null;
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
            string field1 = "";
            if (outField != null)
            {
                field1 = string.Format("{0}{1}", GetPrefix(), __DBAdapter.KeyWordFormat(m1.Member.Name));
            }
            string condition = "";
            var query2 = query.BaseQuery.GetQuery();
            if (expression != null)
            {
                //内部关联
                GetPrefix(typeof(T2));
                string condition2 = FormatJoinExpression(expression.Body);
                query2 += " and " + condition2;
            }
            condition = string.Format("{0} {1}({2})", field1, type, query2);
            if (Condition.Length > 0)
            {
                condition = " and " + condition;
            }
            Condition.Append(condition);
            return this;
        }
        #endregion
        #endregion

        #region 获取解析值
        

        /// <summary>
        /// 是否为关联更新/删除
        /// </summary>
        internal bool _IsRelationUpdate = false;

        /// <summary>
        /// 获取排序 带 order by
        /// </summary>
        /// <returns></returns>
        internal abstract string GetOrderBy();

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
