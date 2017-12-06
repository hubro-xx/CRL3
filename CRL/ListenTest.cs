/**
* CRL 快速开发框架 V4.5
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CoreHelper.SocketUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CRL
{
    public class ListenTestServer : TcpService
    {
        public ListenTestServer(int port) : base(port)
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

        //接收数据并修改部分数据然后发送回去
        void sender_ReceiveData(IDataTransmit sender, NetEventArgs e)
        {
            try
            {
                byte[] data = (byte[])e.EventArg;

                //发送数据
                //sender.Send(data);
                //Log(sender.RemoteEndPoint.ToString() + " 发送数据");
            }
            catch (Exception ex)
            {
                Log("处理数据出错：" + ex.Message);
            }
        }
    }

    public class ListenTestClient
    {
        public static bool Send(string host,int port,string msg)
        {
            IPEndPoint point;
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (Regex.Match(host, @"^[.\d]+$").Success)
                {
                    point = new IPEndPoint(IPAddress.Parse(host), port);
                }
                else
                {
                    var hostEntry = Dns.GetHostEntry(host);
                    point = new IPEndPoint(hostEntry.AddressList[0], port);
                }
                socket.Connect(point);

                byte[] sendBytes = Encoding.Default.GetBytes(msg);
                socket.Send(sendBytes);
                socket.Close();
                return true;
            }
            catch(Exception ero)
            {
                return false;
            }
        }
    }
}
