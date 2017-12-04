using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest.Page
{
    public partial class DbSet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var order = new Code.Order();
            //所有
            var product = order.Products.ToList();

            //返回关联过的查询
            var product2 = order.Products.GetQuery();

            var p = new Code.ProductData() { BarCode = "33333" };
            //添加一项
            order.Products.Add(p);

            order.Products.Delete(p);//删除一项

            //返回完整的BaseProvider
            var provider = order.Products.GetProvider();
        }
    }
}