/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CRL.LambdaQuery;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.DBExtend.MongoDB
{
    public sealed partial class MongoDB
    {

        public override int Delete<T>(LambdaQuery.LambdaQuery<T> query1)
        {
            var query = query1 as MongoDBLambdaQuery<T>;
            var collection = _MongoDB.GetCollection<T>(query.QueryTableName);
            var result = collection.DeleteMany(query.__MongoDBFilter);
            return (int)result.DeletedCount;
        }

        public override int Delete<TModel, TJoin>(System.Linq.Expressions.Expression<Func<TModel, TJoin, bool>> expression)
        {
            throw new NotSupportedException();
        }

        public override int Delete<TModel>(System.Linq.Expressions.Expression<Func<TModel, bool>> expression)
        {
            var query = new MongoDBLambdaQuery<TModel>(dbContext);
            query.Where(expression);
            var collection = _MongoDB.GetCollection<TModel>(query.QueryTableName);
            var result = collection.DeleteMany(query.__MongoDBFilter);
            return (int)result.DeletedCount;
        }

        public override int Delete<TModel>(object id)
        {
            var table = TypeCache.GetTable(typeof(TModel));
            var collection = _MongoDB.GetCollection<TModel>(table.TableName);
            var builder = Builders<TModel>.Filter;
            var filter = builder.Eq(table.PrimaryKey.MemberName, id);
            var result = collection.DeleteMany(filter);
            return (int)result.DeletedCount;

            //var expression = Base.GetQueryIdExpression<TModel>(id);
            //return Delete<TModel>(expression);
        }

    }
}
