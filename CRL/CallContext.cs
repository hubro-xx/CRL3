using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL
{
    /// <summary>
    /// 当前调用上下文
    /// </summary>
    public class CallContext
    {
        public static T GetData<T>(string contextName)
        {
            //return default(T);
            var dbContextObj = System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(contextName);
            if (dbContextObj == null)
                return default(T);
            return (T)dbContextObj;
        }
        public static void SetData(string contextName, object data)
        {
            //return;
            System.Runtime.Remoting.Messaging.CallContext.LogicalSetData(contextName, data);
        }
    }
}
