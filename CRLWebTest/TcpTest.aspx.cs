using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest
{
    public partial class TcpTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var num = Convert.ToInt32(TextBox2.Text);
            Test(TextBox1.Text, 1438, num);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var num = Convert.ToInt32(TextBox2.Text);
            Test(TextBox1.Text, 1437, num);
        }
        void Test(string host, int port, int threadCount)
        {
            for (int i = 0; i < threadCount; i++)
            {
                var thread = new System.Threading.Thread(() =>
                {
                    for (int n = 0; n < 10; n++)
                    {
                        var a = CRL.ListenTestClient.Send(host, port, "test msg");
                    }
                });
                thread.Start();
            }

        }
    }
}