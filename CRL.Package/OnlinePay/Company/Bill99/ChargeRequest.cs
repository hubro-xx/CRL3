/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
namespace CRL.Package.OnlinePay.Company.Bill99
{
    public class ChargeRequest : MessageBase
    {
        string _inputCharset = "1";
        public string inputCharset
        {
            get
            {
                return _inputCharset;
            }
            set
            {
                _inputCharset = value;
            }
        }
        public string bgUrl
        {
            get;
            set;
        }
        string _version = "v2.0";
        public string version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }
        public string language
        {
            get;
            set;
        }
        string _signType = "4";
        public string signType
        {
            get
            {
                return _signType;
            }
            set
            {
                _signType = value;
            }
        }
        public string merchantAcctId
        {
            get;
            set;
        }
        public string payerName
        {
            get;
            set;
        }
        public string payerContactType
        {
            get;
            set;
        }
        public string payerContact
        {
            get;
            set;
        }
        public string orderId
        {
            get;
            set;
        }
        public string orderAmount
        {
            get;
            set;
        }
        public string orderTime
        {
            get;
            set;
        }
        public string productName
        {
            get;
            set;
        }
        public string productNum
        {
            get;
            set;
        }
        public string productId
        {
            get;
            set;
        }
        public string productDesc
        {
            get;
            set;
        }
        public string ext1
        {
            get;
            set;
        }
        public string ext2
        {
            get;
            set;
        }
        public string payType
        {
            get;
            set;
        }
        public string redoFlag
        {
            get;
            set;
        }
        public string pid
        {
            get;
            set;
        }

        
    }
}
