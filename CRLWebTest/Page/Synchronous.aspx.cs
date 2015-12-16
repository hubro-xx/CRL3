using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest
{
    public partial class Synchronous : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //创建表并检查字段
            Code.ProductDataManage.Instance.CreateTable();
            //检查表索引
            Code.ProductDataManage.Instance.CreateTableIndex();
        }
    }
}