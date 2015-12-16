using System;
using System.Collections.Generic;
using System.Web;

namespace CRL.Package.OnlinePay.Company.Weixin
{
    public class WxPayException : Exception 
    {
        public WxPayException(string msg) : base(msg) 
        {

        }
     }
}