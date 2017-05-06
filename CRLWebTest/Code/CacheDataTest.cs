using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTest.Code
{
    public class CacheDataTest : CRL.IModelBase
    {
        public string Name
        {
            get; set;
        }
        protected override System.Collections.IList GetInitData()
        {
            var list = new List<CacheDataTest>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(new CacheDataTest() { Name = "name" + i, });
            }
            return list;
        }
    }
    public class CacheDataTestManage : CRL.BaseProvider<CacheDataTest>
    {
        public static CacheDataTestManage Instance
        {
            get
            {
                return new CacheDataTestManage();
            }
        }
        /// <summary>
        /// 对象被更新时,是否通知缓存服务器
        /// </summary>
        protected override bool OnUpdateNotifyCacheServer
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 是否从远程查询缓存
        /// </summary>
        protected override bool QueryCacheFromRemote
        {
            get
            {
                return true;
            }
        }
    }
}