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
        static int a = 0;
        static int b = 0;
        Type _serverType;
        public AopProxy(Type serverType)
            : base(serverType)
        {
            _serverType = serverType;
            a += 1;
        }
        public override IMessage Invoke(IMessage msg)
        {
            b += 1;
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
                object[] copiedArgs = Array.CreateInstance(typeof(object), callMsg.Args.Length) as object[];
                callMsg.Args.CopyTo(copiedArgs, 0);
                //var copiedArgs = callMsg.Args;
                IMessage message;
                try
                {
                    var taget = GetUnwrappedServer();
                    object returnValue;
                    if (callMsg.MethodName.StartsWith("set_") && copiedArgs.Length == 1)
                    {
                        var method = _serverType.GetMethod("SetChanges", BindingFlags.NonPublic | BindingFlags.Instance);
                        method.Invoke(taget, new object[] { callMsg.MethodName.Substring(4), copiedArgs[0] });
                    }
                    returnValue = callMsg.MethodBase.Invoke(taget, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null,
                        copiedArgs, null);
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
