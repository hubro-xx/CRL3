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

namespace CRL.CacheServer
{
    public class CacheServer
    {
        public static string Query(string json)
        {
            var obj = LambdaQuery.CRLQueryExpression.FromJson(json);
            return Query(obj);
        }
        public static string Query(LambdaQuery.CRLQueryExpression queryExpression)
        {
            if (SettingConfig.ExpressionQueryData == null)
            {
                throw new Exception("请实现SettingConfig.ExpressionQueryData");
            }
            var data = SettingConfig.ExpressionQueryData(queryExpression);
            var result = new ResultData() { Data = data, Total = queryExpression.Total };
            return CoreHelper.StringHelper.SerializerToJson(result);
        }
    }
}
