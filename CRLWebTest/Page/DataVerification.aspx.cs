using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest
{
    public partial class DataVerification : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //ProductData 限定了 BarCode不能为空
            //这里设为空,提交时会抛出异常
            var item = new Code.ProductData() { InterFaceUser = "2222", ProductName = "product2", BarCode = "" };
            var msg = item.CheckData();
            if (!string.IsNullOrEmpty(msg))//手动判断对象数据是否合法
            {
                Response.Write(msg);
            }
            try
            {
                Code.ProductDataManage.Instance.Add(item);
            }
            catch(Exception ero)//捕获异常
            {
                Response.Write(ero.Message);
            }
        }
    }
}