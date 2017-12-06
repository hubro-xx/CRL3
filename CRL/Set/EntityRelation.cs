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
using System.Text;
using System.Threading.Tasks;

namespace CRL
{
    public class EntityRelation<T> where T : IModel, new()
    {
        internal object mainValue = null;

        Expression<Func<T, bool>> _relationExp;
        internal EntityRelation(Expression<Func<T, object>> member, object key, Expression<Func<T, bool>> expression = null)
        {
            mainValue = key;
            Expression relationExpression;
            var parameterExpression = member.Parameters.ToArray();
            if (member.Body is UnaryExpression)
            {
                relationExpression = ((UnaryExpression)member.Body).Operand;
            }
            else
            {
                relationExpression = member.Body;
            }
            var constant = Expression.Constant(mainValue);
            var body = Expression.Equal(relationExpression, constant);
            _relationExp = Expression.Lambda<Func<T, bool>>(body, parameterExpression);
            if (expression != null)
            {
                _relationExp = _relationExp.AndAlso(expression);
            }
        }
        DbContext getDbContext()
        {
            var dbLocation = new CRL.DBLocation() { DataAccessType = DataAccessType.Read, ManageType = typeof(T) };
            var helper = SettingConfig.GetDbAccess(dbLocation);
            var dbContext = new DbContext(helper, dbLocation);
            return dbContext;
        }
        /// <summary>
        /// 获取当前值
        /// </summary>
        /// <returns></returns>
        public T Value
        {
            get
            {
                var dbContext = getDbContext();
                var db = DBExtendFactory.CreateDBExtend(dbContext);
                var item = db.QueryItem(_relationExp);
                return item;
            } 
        }
    }
}
