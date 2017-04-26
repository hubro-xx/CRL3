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
using System.Threading;

namespace CRL.CacheServer
{
    /// <summary>
    /// 连接池形式的Client
    /// </summary>
    internal class TcpPoolClient : CacheClientProxy
    {
        string server;
        int port;
        int minConnections = 5;
        List<Connection> Connections = new List<Connection>();
        Thread thread;
        System.Text.Encoding encode = System.Text.Encoding.UTF8;
        /// <summary>
        /// 连接池形式的Client
        /// </summary>
        /// <param name="_server"></param>
        /// <param name="_port"></param>
        public TcpPoolClient(string _server,int _port)
        {
            server = _server;
            port = _port;
            thread = new Thread(new ThreadStart(threadStart));
            thread.Start();
        }

        public override string Host
        {
            get
            {
                return string.Format("{0}:{1}", server, port);
            }
        }
        static object lockObj = new object();
        #region 超时连接处理
        void threadStart()
        {
            while (true)
            {
                var time = DateTime.Now.AddMinutes(-10);
                lock (lockObj)
                {
                    if (Connections.Count() > minConnections)
                    {
                        var all = Connections.FindAll(b => b.LastUseTime < time);
                        foreach (var item in all)
                        {
                            item.Socket.Dispose();
                            Connections.Remove(item);
                        }
                    }
                }
                System.Threading.Thread.Sleep(1000 * 10);
            }
        }
        #endregion
        public override string SendQuery(string query)
        {
            var data = encode.GetBytes(query);
            if (Connections.Count > 500)
            {
                throw new CRLException("TcpClientPool连接已超过500");
            }
            Connection connection;
            lock (lockObj)
            {
                connection = Connections.Find(b => b.Used == false);
                if (connection == null)
                {
                    connectionIndex += 1;
                    connection = new Connection() { Socket = new CoreHelper.SocketUtil.TcpClient(server, port, 3, true), Index = connectionIndex };
                    Connections.Add(connection);
                }
                connection.Used = true;
            }
            var result = connection.Socket.SendAndReceive(data);
            lock (lockObj)
            {
                connection.Used = false;
                connection.LastUseTime = DateTime.Now;
            }
            if (result == null)
            {
                //connection.Socket.Dispose();
                //Connections.Remove(connection);
                throw new CRLException("连接到缓存服务器时发生错误:" + connection.Socket.LastException.Message);
            }
            var response = encode.GetString(result);
            return response;
        }
        public override void Dispose()
        {
            foreach(var item in Connections)
            {
                item.Socket.Dispose();
            }
            thread.Abort();
        }
        #region obj
        int connectionIndex = 0;
        class Connection
        {
            public override string ToString()
            {
                return string.Format("{0} {1}", Used, Index);
            }
            public bool Used;
            public int Index = 0;
            public CoreHelper.SocketUtil.TcpClient Socket;
            public DateTime LastUseTime = DateTime.Now;
        }
        #endregion
    }
}
