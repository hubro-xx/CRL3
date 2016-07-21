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
