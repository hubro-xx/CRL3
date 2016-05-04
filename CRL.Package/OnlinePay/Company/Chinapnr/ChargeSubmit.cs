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
    public class ChargeSubmit : MessageBase
    {
        public string Version="10";
        public string CmdId = "Buy";
        public string MerId;
        public string OrdId;
        public string OrdAmt;
        public string CurCode;
        public string Pid;
        public string RetUrl;
        public string MerPriv;
        public string GateId;
        public string UsrMp;
        public string DivDetails;
        public string OrderType;
        public string PayUsrId;
        public string PnrNum;
        public string BgRetUrl;
        public string IsBalance;
        public string ChkValue;
    }
}
