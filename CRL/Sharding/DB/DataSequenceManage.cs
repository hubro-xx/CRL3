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

namespace CRL.Sharding.DB
{
    public class DataSequenceManage : CRL.BaseProvider<DataSequence>
    {
        public static DataSequenceManage Instance
        {
            get { return new DataSequenceManage(); }
        }
        /// <summary>
        /// 获取主数据表自增
        /// </summary>
        /// <returns></returns>
        public int GetSequence()
        {
            string sql = "update DataSequence set Sequence=Sequence+1 select Sequence from DataSequence";
            return DBExtend.ExecScalar<int>(sql);
        }
    }
}
