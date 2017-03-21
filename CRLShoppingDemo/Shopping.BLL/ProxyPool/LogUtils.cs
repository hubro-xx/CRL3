using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.BLL.ProxyPool
{
    public class LogUtils
    {
        public static void ErrorLog(Exception ero)
        {
            CoreHelper.EventLog.Log(ero.ToString(), "", false);
        }
    }
}