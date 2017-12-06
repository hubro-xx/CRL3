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

namespace Shopping.BLL.ProxyPool
{
    public class GetProxyJob : CoreHelper.QuartzScheduler.QuartzJob
    {
        public GetProxyJob()
        {
            this.RepeatInterval = new TimeSpan(0, 5, 30);
        }
        public override void DoWork()
        {
            new IpPoolSpider().Initial();
        }
    }
}
