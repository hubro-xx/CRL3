using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Attribute
{
    public class ModelProxyAttribute : System.Runtime.Remoting.Proxies.ProxyAttribute
    {
        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            AopProxy realProxy = new AopProxy(serverType);
            if (!SettingConfig.UseAopProxy)
            {
                SettingConfig.UseAopProxy = true;
            }
            return realProxy.GetTransparentProxy() as MarshalByRefObject;
        }
    }
}
