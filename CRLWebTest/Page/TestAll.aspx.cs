using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest.Page
{
    public partial class TestAll : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Code.TestAll.TestQuery();
            Response.Write("OK");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Code.TestAll.TestUpdate();
            Response.Write("OK");
        }
    }
}