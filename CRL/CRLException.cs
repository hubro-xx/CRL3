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
using System.Text;
using System.Threading.Tasks;

namespace CRL
{
    [Serializable]
    internal class CRLException : Exception
    {
        public CRLException()
            : this("发生异常")
        {
        }

        public CRLException(string message)
            : base(message)
        {
        }

        public CRLException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        public CRLException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
