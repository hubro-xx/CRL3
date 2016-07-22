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

namespace CRL.LambdaQuery
{
    public abstract partial class LambdaQuery<T> where T : IModel, new()
    {
        #region 获取一条记录

        /// <summary>
        /// 获取一条
        /// </summary>
        /// <returns></returns>
        public dynamic ToSingleDynamic()
        {
            return Top(1).Page(0, 0).ToDynamic().FirstOrDefault();
        }
        /// <summary>
        /// 获取一条
        /// </summary>
        /// <returns></returns>
        public T ToSingle()
        {
            return Top(1).Page(0, 0).ToList().FirstOrDefault();
        }
        /// <summary>
        /// 获取一条
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TResult ToSingle<TResult>()where TResult : class,new()
        {
            return Top(1).Page(0, 0).ToList<TResult>().FirstOrDefault();
        }
        #endregion

        /// <summary>
        /// 返回动态对象
        /// 会按GROUP和分页判断
        /// </summary>
        /// <returns></returns>
        public List<dynamic> ToDynamic()
        {
            var db = DBExtendFactory.CreateDBExtend(__DbContext);
            if (SkipPage > 0)
            {
                return db.Page(this);
            }
            return db.QueryDynamic(this);
        }
        /// <summary>
        /// 按select返回匿名对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public List<TResult> ToDynamic<TResult>(Expression<Func<T, TResult>> resultSelector)
        {
            //只能做到当前对象筛选
            var db = DBExtendFactory.CreateDBExtend(__DbContext);
            return db.QueryDynamic(this, resultSelector);
        }
        /// <summary>
        /// 返回指定类型
        /// 会按GROUP和分页判断
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public List<TResult> ToList<TResult>()
            where TResult : class,new()
        {
            var db = DBExtendFactory.CreateDBExtend(__DbContext);
            if (SkipPage > 0)
            {
                return db.Page<T, TResult>(this);
            }
            return db.QueryDynamic<T, TResult>(this);
        }
        /// <summary>
        /// 返回当前类型
        /// 会按GROUP和分页判断
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            var db = DBExtendFactory.CreateDBExtend(__DbContext);
            //如果是筛选后的结果,属性可能匹配不上
            if (SkipPage > 0)
            {
                return db.Page<T, T>(this);
            }
            return db.QueryList(this);
        }
        /// <summary>
        /// 返回字典
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Dictionary<TKey, TValue> ToDictionary<TKey, TValue>()
        {
            var db = DBExtendFactory.CreateDBExtend(__DbContext);
            return db.ToDictionary<T, TKey, TValue>(this);
        }

        #region 返回首列结果
        /// <summary>
        /// 返回首列结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TResult ToScalar<TResult>()
        {
            var db = DBExtendFactory.CreateDBExtend(__DbContext);
            var result = db.QueryScalar(this);
            if (result == null)
            {
                return default(TResult);
            }
            return (TResult)result;
        }
        /// <summary>
        /// 返回首列结果
        /// </summary>
        /// <returns></returns>
        public dynamic ToScalar()
        {
            var db = DBExtendFactory.CreateDBExtend(__DbContext);
            return db.QueryScalar(this);
        }
        #endregion
    }
}
