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

namespace CRL.CacheServer
{
    internal class HttpCacheClient : CacheClientProxy
    {
        public override string Host
        {
            get { return "http://localhost:56640/page/CacheServer.ashx"; }
        }
        public override string SendQuery(string data)
        {
            return CoreHelper.HttpRequest.HttpPost(Host, "q=" + data, System.Text.Encoding.UTF8);
        }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
        
}
