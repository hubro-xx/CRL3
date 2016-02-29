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
