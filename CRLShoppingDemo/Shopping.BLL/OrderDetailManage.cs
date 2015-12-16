using Shopping.Model.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.BLL
{
    public class OrderDetailManage : CRL.BaseProvider<OrderDetail>
    {
        public static OrderDetailManage Instance
        {
            get { return new OrderDetailManage(); }
        }
    }
}
