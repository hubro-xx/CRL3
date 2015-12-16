using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest
{
    public partial class Query1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //查询一项
            var item = Code.ProductDataManage.Instance.QueryItem(b => b.Id > 0 && b.Number == 0);
            var list = new List<object>(); 
            
            if(item!=null)
            {
                list.Add(item);
            }
            GridView1.DataSource = list;
            GridView1.DataBind();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            //查询集合
            var list = Code.ProductDataManage.Instance.QueryList(b => b.Id > 0 && b.Id < 200);
            GridView1.DataSource = list;
            GridView1.DataBind();
        }
    }
}