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

namespace CRL
{
    public class CacheServerSetting
    {
        #region 缓存服务器
        internal static List<string> ServerTypeSetting = new List<string>();
        internal static Dictionary<string, ExpressionDealDataHandler> CacheServerDealDataRules = new Dictionary<string, ExpressionDealDataHandler>();
        /// <summary>
        /// 服务端清加数据处理规则
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handler"></param>
        public static void AddCacheServerDealDataRule(Type type, ExpressionDealDataHandler handler)
        {
            CacheServerDealDataRules.Add(type.FullName, handler);
            ServerTypeSetting.Add(type.FullName);
        }
        #endregion
        /// <summary>
        /// 缓存客户端代理
        /// </summary>
        internal static List<CacheServer.CacheClientProxy> CacheClientProxies = new List<CacheServer.CacheClientProxy>();
        internal static Dictionary<string, CacheServer.CacheClientProxy> ServerTypeSettings = new Dictionary<string, CacheServer.CacheClientProxy>();
        /// <summary>
        /// 添加服务端监听
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public static void AddTcpServerListen(string host, int port)
        {
            var client = new CRL.CacheServer.TcpPoolClient(host, port);
            CacheClientProxies.Add(client);
        }
        /// <summary>
        /// 初始服务端设置,会访问所有服务端获取设置
        /// </summary>
        public static void Init()
        {
            foreach (var p in CacheClientProxies)
            {
                p.GetServerTypeSetting();
            }
        }
        /// <summary>
        /// 释放客户端连接
        /// </summary>
        public static void Dispose()
        {
            foreach (var p in CacheClientProxies)
            {
                p.Dispose();
            }
        }
        internal static CacheServer.CacheClientProxy GetCurrentClient(Type type)
        {
            string typeName = type.FullName;
            if (ServerTypeSettings.ContainsKey(typeName))
            {
                return ServerTypeSettings[typeName];
            }
            return null;
            //throw new CRLException("未在服务器上找到对应的数据处理类型;" + typeName);
        }
    }
}
