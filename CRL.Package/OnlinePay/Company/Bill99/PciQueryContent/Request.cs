using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.OnlinePay.Company.Bill99.PciQueryContent
{
    //PCI查询
    public class Request : PCIBase
    {
        public override string InterfacePath
        {
            get
            {
                return "/cnp/pci_querys";
            }
        } 
    }
}
