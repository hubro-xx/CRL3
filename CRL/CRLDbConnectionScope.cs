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

namespace CRL
{
    /// <summary>
    /// 使数据访问在同一个范围内
    /// 只能同一个库
    /// </summary>
    public class CRLDbConnectionScope : IDisposable
    {
        bool canClose = false;
        Guid name;
        /// <summary>
        /// 使数据访问在同一个范围内
        /// </summary>
        public CRLDbConnectionScope()
        {
            name = System.Guid.NewGuid();
            var _useCRLContext = CallContext.GetData<bool>(Base.UseCRLContextFlagName);//事务已开启,内部事务不用处理
            if (!_useCRLContext)
            {
                CallContext.SetData(Base.UseCRLContextFlagName, true);
                canClose = true;
            }
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (canClose)
            {
                CallContext.SetData(Base.UseCRLContextFlagName, false);
                var db = CallContext.GetData<AbsDBExtend>(Base.CRLContextName);
                if (db != null)
                {
                    db.__DbHelper.CloseConn(true);
                }
            }
        }
    }
}
