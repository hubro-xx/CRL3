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
    /// <summary>
    /// 库
    /// 按主数据分垂直划分,将主数据按不同段存在不同库中
    /// </summary>
    public class DataBase:CRL.IModelBase
    {
        /// <summary>
        /// 库名
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 主数据开始INDEX
        /// </summary>
        public int MainDataStartIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 主数据结束INDEX
        /// </summary>
        public int MainDataEndIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 主数据表最大数据量
        /// </summary>
        public int MaxMainDataTotal
        {
            get;
            set;
        }
        public override string ToString()
        {
            return string.Format("名称:{0} 最大主数据量:{1} 索引开始:{2} 结束{3}", Name, MaxMainDataTotal, MainDataStartIndex, MainDataEndIndex);
        }
    }
}
