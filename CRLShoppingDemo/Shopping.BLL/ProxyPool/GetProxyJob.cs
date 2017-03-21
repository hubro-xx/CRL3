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
