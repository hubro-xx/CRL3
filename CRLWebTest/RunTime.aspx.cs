using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest
{
    public partial class RunTime : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var str = CRL.Runtime.RunTimeService.Display();
            Response.Write(str);
            Response.End();
        }
    }
}