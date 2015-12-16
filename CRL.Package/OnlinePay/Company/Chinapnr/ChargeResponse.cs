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
