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

namespace CRL.DBExtend.MongoDB
{
    public sealed partial class MongoDB
    {
        public override void BatchInsert<TModel>(List<TModel> details, bool keepIdentity = false)
        {
            if (details.Count == 0)
                return;
            string table = TypeCache.GetTableName(typeof(TModel), dbContext);
            var collection = _MongoDB.GetCollection<TModel>(table);
            collection.InsertMany(details);
        }
        public override void InsertFromObj<TModel>(TModel obj)
        {
            string table = TypeCache.GetTableName(typeof(TModel), dbContext);
            var collection = _MongoDB.GetCollection<TModel>(table);
            collection.InsertOne(obj);
        }
    }
}
