/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
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

        public override int Update<TModel, TJoin>(System.Linq.Expressions.Expression<Func<TModel, TJoin, bool>> expression, ParameCollection updateValue)
        {
            throw new NotSupportedException();
        }
        int Update<TModel>(FilterDefinition<TModel> filter, ParameCollection setValue)
        {
            var table = TypeCache.GetTable(typeof(TModel));
            var collection = _MongoDB.GetCollection<TModel>(table.TableName);
            var update = Builders<TModel>.Update;
            var first = setValue.First();
            var updateSet = update.Set(first.Key, first.Value);
            setValue.Remove(first.Key);
            foreach (var item in setValue)
            {
                if (item.Key.StartsWith("$"))
                {
                    throw new CRLException("MongoDB不支持累加" + item.Key);
                }
                update.Set(item.Key, item.Value);
            }
            var result = collection.UpdateMany(filter, updateSet);
            return (int)result.ModifiedCount;

        }
        public override int Update<TModel>(LambdaQuery.LambdaQuery<TModel> query1, ParameCollection setValue)
        {
            if (query1.__GroupFields.Count > 0)
            {
                throw new CRLException("update不支持group查询");
            }
            if (query1.__Relations.Count > 1)
            {
                throw new CRLException("update关联不支持多次");
            }
            if (setValue.Count == 0)
            {
                throw new ArgumentNullException("更新时发生错误,参数值为空 ParameCollection setValue");
            }
            var query = query1 as LambdaQuery.MongoDBLambdaQuery<TModel>;
            return Update(query.__MongoDBFilter, setValue);
        }
        public override int Update<TModel>(TModel obj)
        {
            var c = GetUpdateField(obj);
            if (c.Count == 0)
            {
                return 0;
                //throw new CRLException("更新集合为空");
            }
            var keyValue = obj.GetpPrimaryKeyValue();
            var table = TypeCache.GetTable(typeof(TModel));
            var collection = _MongoDB.GetCollection<TModel>(table.TableName);
            var builder = Builders<TModel>.Filter;
            var filter = builder.Eq(table.PrimaryKey.MemberName, keyValue);
            var n = Update(filter, c);

            //var expression = Base.GetQueryIdExpression<TModel>(keyValue);
            //var n = Update(expression,c);
            //if (n == 0)
            //{
            //    throw new CRLException("更新失败,找不到主键为 " + keyValue + " 的记录");
            //}
            obj.CleanChanges();
            return n;
        }
    }
}
