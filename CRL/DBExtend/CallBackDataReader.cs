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
        System.Data.Common.DbDataReader reader;
        Func<int> handler;
        public double runTime;
        public CallBackDataReader(System.Data.Common.DbDataReader _reader, Func<int> _handler)
        {
            reader = _reader;
            handler = _handler;
        }
        public List<T> GetDataTResult<T>(LambdaQuery.Mapping.QueryInfo<T> queryInfo,out int outParame)
        {
            var data = ObjectConvert.DataReaderToSpecifiedList<T>(reader, queryInfo);
            outParame = handler();
            return data;
        }
        public List<dynamic> GetDataDynamic(out int outParame) 
        {
            double runTime;
            var data = Dynamic.DynamicObjConvert.DataReaderToDynamic(reader, out runTime);
            //reader.Close();
            outParame = handler();
            return data;
        }
    }
}
