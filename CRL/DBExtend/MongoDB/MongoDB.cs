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
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using CRL.LambdaQuery;
namespace CRL.DBExtend.MongoDBEx
{
    public sealed partial class MongoDBExt : AbsDBExtend
    {
        public MongoDBExt(DbContext _dbContext)
            : base(_dbContext)
        {
        }
        protected override LambdaQuery<TModel> CreateLambdaQuery<TModel>()
        {
            return new MongoDBLambdaQuery<TModel>(dbContext);
        }
        IMongoDatabase _mongoDatabase=null;
        
        IMongoDatabase _MongoDB
        {
            get {
                if (_mongoDatabase == null)
                {
                    var db = GetDBHelper();
                    var connectionString = db.ConnectionString;
                    var _client = new MongoClient(connectionString);
                    _mongoDatabase = _client.GetDatabase(db.DatabaseName);
                }
                return _mongoDatabase; }
            set { _mongoDatabase = value; }
        }
        
        internal override TType GetFunction<TType, TModel>(Expression<Func<TModel, bool>> expression, Expression<Func<TModel, TType>> selectField, FunctionType functionType, bool compileSp = false)
        {
            var query = new MongoDBLambdaQuery<TModel>(dbContext);
            query.Select(selectField.Body);
            query.Where(expression);
            var collection = _MongoDB.GetCollection<TModel>(query.QueryTableName);
            object result = null;
            switch (functionType)
            {
                case FunctionType.COUNT:
                    result = collection.Count(query.__MongoDBFilter);
                    break;
                default:
                    throw new NotSupportedException("MongoDB不支持的函数:" + functionType);
            }
            return ObjectConvert.ConvertObject<TType>(result);
        }
    }
}
