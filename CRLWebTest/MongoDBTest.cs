/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace WebTest
{
    public class MongoDBTest
    {
        public static void Test()
        {
            var _client = new MongoClient("mongodb://localhost:27017");
            var _database = _client.GetDatabase("test2");
            var collection = _database.GetCollection<Code.MongoDBModel>("MongoDBModel");
            //var groupInfo = new BsonDocument();
            //var groupInfo2 = new BsonDocument { { "_id", "$OrderId" }, { "count", new BsonDocument("$sum", "$Status") } };
            //groupInfo.Add("_id", "$OrderId");
            //groupInfo.Add("count", new BsonDocument("$sum", "$Status"));
            ////var aggregate = collection.Aggregate().Group(groupInfo);
            //var aggregate = collection.Aggregate().Group(groupInfo);
            ////var aggregate = collection.Aggregate().Group(b => b.Id, b => b.Select(x => new { key = b.Key, count = b.Count() }));
            //var result2 = aggregate.ToList();
            var builder = Builders<Code.MongoDBModel>.Filter;
            var f1 = builder.Regex("OrderId", new BsonRegularExpression("^1212"));
            var f2 = new BsonDocument() { { "OrderId", new BsonRegularExpression("^1212") } };
            var a = "10";
            SortDefinition<Code.MongoDBModel> sort = new BsonDocument();
            sort = sort.Ascending(b => b.Id);
            var query = collection.Find(f1).Sort(sort);
            var result = query.ToList();
            //var sum = result.Sum(b=>b.Status);
        }
    }
}
