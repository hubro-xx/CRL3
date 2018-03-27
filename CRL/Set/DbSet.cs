/**
* CRL 快速开发框架 V4.5
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CRL.LambdaQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Set
{
    public interface IDbSet
    {
        void Save();
    }
    /// <summary>
    /// DbSet结构,增强对象关联性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbSet<T>: IDbSet where T : IModel, new()
    {
        #region inner
        class DbSetProvider: BaseProvider<T>
        {
            //重写了GetLambdaQuery,与关联条件保持一致
            Expression<Func<T, bool>> relationExpression;
            public DbSetProvider(Expression<Func<T, bool>> expression)
            {
                relationExpression = expression;
            }
            public override LambdaQuery<T> GetLambdaQuery()
            {
                return base.GetLambdaQuery().Where(relationExpression);
            }
        }
        #endregion
        internal object mainValue = null;

        Expression<Func<T, bool>> _relationExp;
        string memberName;
        internal DbSet(Expression<Func<T, object>> member, object key, Expression<Func<T, bool>> expression = null)
        {
            mainValue = key;
            MemberExpression relationExpression;
            var parameterExpression = member.Parameters.ToArray();
            if (member.Body is UnaryExpression)
            {
                relationExpression = ((UnaryExpression)member.Body).Operand as MemberExpression;
            }
            else
            {
                relationExpression = member.Body as MemberExpression;
            }
            if (relationExpression == null)
            {
                throw new CRLException("member 不为 MemberExpression");
            }
            memberName = relationExpression.Member.Name;
            var constant = Expression.Constant(mainValue);
            var body = Expression.Equal(relationExpression, constant);
            _relationExp = Expression.Lambda<Func<T, bool>>(body, parameterExpression);
            if (expression != null)
            {
                _relationExp = _relationExp.AndAlso(expression);
            }
            _BaseProvider = new DbSetProvider(_relationExp);
        }

        BaseProvider<T> _BaseProvider;
        /// <summary>
        /// 返回BaseProvider
        /// </summary>
        /// <returns></returns>
        public BaseProvider<T> GetProvider()
        {
            return _BaseProvider;
        }

        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <returns></returns>
        public LambdaQuery<T> GetQuery()
        {
            return _BaseProvider.GetLambdaQuery();
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public LambdaQuery<T> Page(int pageSize, int pageIndex)
        {
            return GetQuery().Page(pageSize, pageIndex);
        }
        /// <summary>
        /// 返回所有
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            return GetQuery().ToList();
        }
        /// <summary>
        /// 按条件返回
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public List<T> Where(Expression<Func<T, bool>> expression)
        {
            return GetQuery().Where(expression).ToList();
        }
        /// <summary>
        /// 查找一个
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Find(object id)
        {
            return _BaseProvider.QueryItem(id);
        }
        internal List<T> addObjs = new List<T>();
        internal List<T> removeObjs = new List<T>();
        internal List<T> updateObjs = new List<T>();
        /// <summary>
        /// 添加
        /// 需调用Save保存更改
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            addObjs.Add(item);
            //_BaseProvider.Add(item);
        }
        /// <summary>
        /// 删除一项
        /// 需调用Save保存更改
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            removeObjs.Add(item);
            //return _BaseProvider.Delete(item);
        }
        /// <summary>
        /// 保存更改
        /// </summary>
        public void Save()
        {
            _BaseProvider.Add(addObjs);
            foreach(var item in removeObjs)
            {
                _BaseProvider.Delete(item);
            }
            foreach (var item in updateObjs)
            {
                _BaseProvider.Update(item);
            }
            addObjs.Clear();
            removeObjs.Clear();
            updateObjs.Clear();
        }
        /// <summary>
        /// 删除所有
        /// </summary>
        /// <returns></returns>
        public int RemoveAll()
        {
            return _BaseProvider.Delete(_relationExp);
        }
        /// <summary>
        /// 更改
        /// 需调用Save保存更改
        /// </summary>
        /// <param name="item"></param>
        public void Update(T item)
        {
            updateObjs.Add(item);
            //return _BaseProvider.Update(item);
        }
        #region 函数
        public TType Sum<TType>(Expression<Func<T, bool>> expression, Expression<Func<T, TType>> field, bool compileSp = false)
        {
            return _BaseProvider.Sum(expression,field,compileSp);
        }
        public int Count(Expression<Func<T, bool>> expression, bool compileSp = false)
        {
            return _BaseProvider.Count(expression, compileSp);    
        }
        #endregion
    }
}
