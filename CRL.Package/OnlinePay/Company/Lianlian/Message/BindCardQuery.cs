using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.OnlinePay.Company.Lianlian.Message
{
    public class BindCardQuery:MessageBase
    {
        public override string InterFaceUrl
        {
            get
            {
                return "https://yintong.com.cn/traderapi%20/userbankcard.htm";
            }
        }
        public string user_id;
        public string platform;
        public string pay_type = "D";
        /// <summary>
        /// 签约协议号
        /// </summary>
        public string no_agree;
        public string card_no;
        public string offset = "0";
    }
}
