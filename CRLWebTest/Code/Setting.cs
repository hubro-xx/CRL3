using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics; 
namespace WebTest.Code
{
    public class Setting
    {
        public static string GetVersion()
        {
            return CRL.Base.GetVersion().ToString();
            string path = HttpContext.Current.Server.MapPath("/bin/CRL.dll");
            FileVersionInfo myFileVersion = FileVersionInfo.GetVersionInfo(path);
            return myFileVersion.FileVersion; 
        
        }
        public static string Value1 = GetVersion();
    }
}