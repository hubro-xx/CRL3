using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest.Page
{
    public partial class SpeedTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ChloePerformanceTest.CRLManage.Instance.QueryItem(b => b.Id > 0);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            var array = typeof(ChloePerformanceTest.MappingSpeedTest).GetMethods(BindingFlags.Static | BindingFlags.Public);
            var instance = new Code.TestAll();
            var n = Convert.ToInt32(TextBox1.Text);
            long useTime;
            string txt = "查询 top " + n + "行数据\r\n";
            foreach (var item in array)
            {
                item.Invoke(null, new object[] { 1 });
                useTime = ChloePerformanceTest.SW.Do(() =>
                {
                    item.Invoke(null, new object[] { n });
                });
                GC.Collect();
                txt += string.Format("{0}用时:{1}ms\r\n", item.Name, useTime);
            }
            TextBox2.Text = txt;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            TextBox1.Text = "200";
            var array = typeof(ChloePerformanceTest.MappingSpeedTest).GetMethods(BindingFlags.Static | BindingFlags.Public);
            var instance = new Code.TestAll();
            var n = Convert.ToInt32(TextBox1.Text);
            long useTime;
            string txt = "top 1 轮循" + n + "次\r\n";
            foreach (var item in array)
            {
                item.Invoke(null, new object[] { 1 });
                useTime = ChloePerformanceTest.SW.Do(() =>
                {
                    for (int i = 0; i < n; i++)
                    {
                        item.Invoke(null, new object[] { 1 });
                    }
                });
                GC.Collect();
                txt += string.Format("{0}用时:{1}ms\r\n", item.Name, useTime);
            }
            TextBox2.Text = txt;
        }
    }
}