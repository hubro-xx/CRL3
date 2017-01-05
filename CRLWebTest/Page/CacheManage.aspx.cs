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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest.Page
{
    public partial class CacheManage : System.Web.UI.Page
    {
        public List<CRL.MemoryDataCache.QueryItem> caches=new List<CRL.MemoryDataCache.QueryItem>();
        public Dictionary<string, int> tempCache = new Dictionary<string, int>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["type"] == "update")
            {
                Update();
                Response.End();
            }
            var all = Code.ProductDataManage.Instance.AllCache;//调用缓存,让它生成
            caches = CRL.MemoryDataCache.CacheService.GetCacheList();
            tempCache = CRL.Base.GetTempCacheCount();
        }
        void Update()
        {
            string key = Request["key"];
            var time = DateTime.Now;
            var a = CRL.MemoryDataCache.CacheService.UpdateCache(key);
            var ts = DateTime.Now - time;
            var str = string.Format("更新 {0},用时 {1}", a, ts);
            Response.Write(str);
        }
    }
}
