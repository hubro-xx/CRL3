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
using System.Reflection;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;

namespace CRL.Attribute
{
    public class AopProxy : System.Runtime.Remoting.Proxies.RealProxy
    {
        Type _serverType;
        public AopProxy(Type serverType)
            : base(serverType)
        {
            _serverType = serverType;
        }
        public override IMessage Invoke(IMessage msg)
        {
            if (msg is IConstructionCallMessage)
            {
                IConstructionCallMessage constructCallMsg = msg as IConstructionCallMessage;
                IConstructionReturnMessage constructionReturnMessage = this.InitializeServerObject(constructCallMsg);
                RealProxy.SetStubData(this, constructionReturnMessage.ReturnValue);
                return constructionReturnMessage;
            }
            else if (msg is IMethodCallMessage)
            {
                IMethodCallMessage callMsg = msg as IMethodCallMessage;
                //object[] copiedArgs = Array.CreateInstance(typeof(object), callMsg.Args.Length) as object[];
                //callMsg.Args.CopyTo(copiedArgs, 0);
                var copiedArgs = callMsg.Args;
                IMessage message;
                try
                {
                    var taget = GetUnwrappedServer();
                    if (taget is IModel)
                    {
                        if (callMsg.MethodName.StartsWith("set_") && copiedArgs.Length == 1)
                        {
                            //透明代理无法调试
                            //var model = taget as IModel;
                            //if (model.GetInnerChanges())
                            //{
                            //    model.SetChanges(callMsg.MethodName.Substring(4), copiedArgs[0]);
                            //}
                        }
                    }
                    var returnValue = callMsg.MethodBase.Invoke(taget, copiedArgs);
                    message = new ReturnMessage(returnValue, copiedArgs, copiedArgs.Length, callMsg.LogicalCallContext, callMsg);
                }
                catch (Exception e)
                {
                    message = new ReturnMessage(e, callMsg);
                }
                return message;
            }
            return msg;

        }
    }
}
