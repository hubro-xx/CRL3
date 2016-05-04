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

namespace CRL.Package.OnlinePay.Company.Chinapnr
{
    public class ChargeResponse : MessageBase
    {
        public string CmdId;
        public string MerId;
        public string RespCode;
        public string TrxId;
        public string OrdAmt;
        public string CurCode;
        public string Pid;
        public string OrdId;
        public string MerPriv;
        public string RetType;
        public string DivDetails;
        public string GateId;
        public string ChkValue;
    }
}
