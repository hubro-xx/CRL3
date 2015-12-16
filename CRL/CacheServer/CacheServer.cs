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
