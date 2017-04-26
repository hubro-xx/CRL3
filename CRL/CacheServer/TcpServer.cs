/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CoreHelper.SocketUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace CRL.CacheServer
{
    /// <summary>
    /// Tcp服务端
    /// </summary>
    public class TcpServer : CoreHelper.SocketUtil.TcpService
    {
        public TcpServer(int port)
            : base(port)
        {
            Connected += new NetEventHandler(server_Connected);
            DisConnect += new NetEventHandler(server_DisConnect);
        }
        
        void server_DisConnect(IDataTransmit sender, NetEventArgs e)
        {
            //Log(sender.RemoteEndPoint.ToString() + " 连接断开");
        }

        void server_Connected(IDataTransmit sender, NetEventArgs e)
        {
            //Log(sender.RemoteEndPoint.ToString() + " 连接成功");
            sender.ReceiveData += new NetEventHandler(sender_ReceiveData);
            //接收数据
            sender.Start();
        }
        System.Text.Encoding encode = System.Text.Encoding.UTF8;
        //接收数据并修改部分数据然后发送回去
        void sender_ReceiveData(IDataTransmit sender, NetEventArgs e)
        {
            string result;
            try
            {
                byte[] data = (byte[])e.EventArg;
                var json = encode.GetString(data);
                result = CRL.CacheServer.CacheService.Deal(json);
            }
            catch (Exception ex)
            {
                Log("处理数据出错：" + ex.Message);
                result = "error,服务器内部错误:" + ex.Message;
            }
            if (result.Length > 1024 * 50)
            {
                result = "error,返回数据长度超过了50K,数据可能无法返回,请缩小查询范围";
            }
            //发送数据
            var data2 = encode.GetBytes(result);
            sender.Send(data2);
        }
    }
}
