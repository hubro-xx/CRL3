/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CRL.LambdaQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
namespace CRL.DBExtend.MongoDB
{
    public sealed partial class MongoDB
    {
        //public override List<TResult> Page<TResult>(LambdaQuery.LambdaQueryBase query1)
        //{
        //    throw new NotSupportedException();
        //    //var result = QueryList(query1);
        //    //if (typeof(TResult) == typeof(TModel))
        //    //{
        //    //    return result as List<TResult>;
        //    //}
        //    //return result.ToType<TResult>();
        //}
        //public override List<TModel> Page<TModel>(LambdaQuery<TModel> query)
        //{
        //    var result = QueryList(query);
        //    return result;
        //}
        //public override List<dynamic> PageDynamic<TModel>(LambdaQuery.LambdaQuery<TModel> query1)
        //{
        //    return GetDynamicResult(query1);
        //}

        
    }
}
