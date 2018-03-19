/**
* CRL 快速开发框架 V4.5
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CRL
{
    /// <summary>
    /// 可回调取出out参数的DataReader
    /// </summary>
    internal class CallBackDataReader
    {
        public System.Data.Common.DbDataReader reader;
        public string Sql;
        Func<int> handler;
        public double runTime = 0;
        public CallBackDataReader(System.Data.Common.DbDataReader _reader, Func<int> _handler,string sql)
        {
            reader = _reader;
            handler = _handler;
            Sql = sql;
        }
        public List<T> GetDataTResult<T>(LambdaQuery.Mapping.QueryInfo<T> queryInfo,out int outParame)
        {
            var data = ObjectConvert.DataReaderToSpecifiedList<T>(reader, queryInfo);
            outParame = handler();
            reader.Close();
            reader.Dispose();
            return data;
        }
        public List<dynamic> GetDataDynamic(out int outParame) 
        {
            double runTime;
            var data = Dynamic.DynamicObjConvert.DataReaderToDynamic(reader, out runTime);
            //reader.Close();
            outParame = handler();
            reader.Close();
            reader.Dispose();
            return data;
        }
    }
}
