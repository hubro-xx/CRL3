using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest
{
    public partial class OrderTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var order = new Code.Order() { UserId = 1, Remark = "test" };
            order.Channel = "channel1";//Channel是新增加的属性
            //通过继承实现新增业务,不需要再写提交订单方法
            //Code.OrderManage.Instance.SubmitOrder(order);
        }
    }
}