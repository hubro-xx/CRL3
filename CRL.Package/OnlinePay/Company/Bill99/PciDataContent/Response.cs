using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CRL.Package.OnlinePay.Company.Bill99.PciDataContent
{
    public class Response : PCIBase
    {
        /// <summary>
        /// 缩略卡号
        /// </summary>
        public string storablePan;
        public static Response FromXml(XmlDocument xmlDoc)
        {
            var obj = new Response();
            obj.customerId = xmlDoc.SelectSingleNode("MasMessage/PciDataContent/customerId").InnerText;
            obj.storablePan = xmlDoc.SelectSingleNode("MasMessage/PciDataContent/storablePan").InnerText;
            obj.responseCode = xmlDoc.SelectSingleNode("MasMessage/PciDataContent/responseCode").InnerText;
            return obj;
        }
    }
}
