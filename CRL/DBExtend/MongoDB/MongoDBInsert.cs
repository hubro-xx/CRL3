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
using System.Threading.Tasks;

namespace CRL.DBExtend.MongoDBEx
{
    public sealed partial class MongoDBExt
    {
        public override void BatchInsert<TModel>(List<TModel> details, bool keepIdentity = false)
        {
            if (details.Count == 0)
                return;
            var table = TypeCache.GetTable(typeof(TModel));
            var collection = _MongoDB.GetCollection<TModel>(table.TableName);
            foreach(var item in details)
            {
                var index = getId(table.TableName);
                table.PrimaryKey.SetValue(item, index);
            }
            collection.InsertMany(details);
        }
        int getId(string tableName)
        {
            var newIndex = _MongoDB.RunCommand<MongoDB.Bson.BsonDocument>(@"{findAndModify:'ids',query:{_id:'" + tableName + @"'}, update:{
$inc:{ 'currentIdValue':1}
        }, new:true,upsert:true}");
            var index = newIndex["value"]["currentIdValue"].AsInt32;
            return index;
        }
        public override void InsertFromObj<TModel>(TModel obj)
        {
            var table = TypeCache.GetTable(typeof(TModel));
            var index = getId(table.TableName);
            table.PrimaryKey.SetValue(obj, index);
            var collection = _MongoDB.GetCollection<TModel>(table.TableName);
            collection.InsertOne(obj);
        }
    }
}
