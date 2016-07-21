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
        public List<T> GetData<T>(out int outParame) where T : class,new()
        {
            var data = ObjectConvert.DataReaderToList<T>(reader,out runTime, false);
            reader.Close();
            outParame = handler();
            return data;
        }
        public List<dynamic> GetDataDynamic(out int outParame) 
        {
            double runTime;
            var data = Dynamic.DynamicObjConvert.DataReaderToDynamic(reader, out runTime);
            reader.Close();
            outParame = handler();
            return data;
        }
        public List<TResult> GetDataDynamic<T, TResult>(Expression<Func<T, TResult>> resultSelector, out int outParame) where T : IModel, new()
        {
            double runTime;
            var data = Dynamic.DynamicObjConvert.DataReaderToDynamic(reader, resultSelector, out runTime);
            reader.Close();
            outParame = handler();
            return data;
        }
    }
}
