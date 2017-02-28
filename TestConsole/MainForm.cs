using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SqlSugar;
namespace TestConsole
{
    public partial class MainForm : Form
    {
        Dictionary<string, Action<int>> methods = new Dictionary<string, Action<int>>();
        public MainForm()
        {
            InitializeComponent();
            methods.Add("DapperQueryTest(SQL)", MappingSpeedTest.DapperQueryTest);
            methods.Add("SugarQueryTest", MappingSpeedTest.SugarQueryTest);
            methods.Add("LoognQueryTest(SQL)", MappingSpeedTest.LoognQueryTest);

            methods.Add("ChloeQueryTest", MappingSpeedTest.ChloeQueryTest);
            methods.Add("EFLinqQueryTest", MappingSpeedTest.EFLinqQueryTest);
            methods.Add("EFSqlQueryTest(SQL)", MappingSpeedTest.EFSqlQueryTest);
            methods.Add("LinqToDBQueryTest", MappingSpeedTest.LinqToDBQueryTest);

           
            methods.Add("CRLQueryTest", MappingSpeedTest.CRLQueryTest);
            methods.Add("CRLSQLQueryTest(SQL)", MappingSpeedTest.CRLSQLQueryTest);
        }
        private async void MainForm_Load(object sender, EventArgs e)
        {
            //TestConsole.CRLManage.Instance.QueryItem(b => b.Id > 0);
            //var obj = CRL.Base.CreateObjectTest<TestEntity>();
            //MappingSpeedTest.LinqToDBQueryTest(1); 

            //button1.Enabled = false;
            //button2.Enabled = false;
            //await Task.Run(() =>
            //{
            //    TestConsole.CRLManage.Instance.QueryItem(b => b.Id > 0);
            //    labTip.Visible = false;
            //    button1.Enabled = true;
            //    button2.Enabled = true;
            //});
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            var n = Convert.ToInt32(textBox1.Text);
            string txt = "查询 top " + n + "行数据\r\n";
            txtResult.Clear();
            txtResult.AppendText(txt);

            await Task.Run(() =>
            {
                long useTime;
                foreach (var kv in methods)
                {
                    var item = kv.Value;
                    item(1);
                    useTime = TestConsole.SW.Do(() =>
                    {
                        item(n);
                    });
                    GC.Collect();
                    txt = string.Format("{0}用时:{1}ms\r\n", kv.Key, useTime);
                    txtResult.AppendText(txt);
                }
                button1.Enabled = true;
                button2.Enabled = true;
            });
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            var n = Convert.ToInt32(textBox2.Text);
            string txt = "top 1 轮循" + n + "次\r\n";
            txtResult.Clear();
            txtResult.AppendText(txt);
            await Task.Run(() =>
            {
                long useTime;

                foreach (var kv in methods)
                {
                    var item = kv.Value;
                    item(1);
                    useTime = TestConsole.SW.Do(() =>
                    {
                        for (int i = 0; i < n; i++)
                        {
                            item(1);
                        }
                    });
                    GC.Collect();
                    txt = string.Format("{0}用时:{1}ms\r\n", kv.Key, useTime);
                    txtResult.AppendText(txt);
                }
                button1.Enabled = true;
                button2.Enabled = true;
            });
        }
        static int id = 20;
        static int id2 = 30;
        private void txtResult_DoubleClick(object sender, EventArgs e)
        {
            
        }
    
        public class TestClass
        {
            public int Id
            {
                get;
                set;
            }
            public string Name
            {
                get;
                set;
            }
        }
        private void btnTest_Click(object sender, EventArgs e)
        {
            var n = Convert.ToInt32(textBox2.Text);
            //n = 3;
            //MappingSpeedTest.CRLQueryTest(1);
            var sw = new Stopwatch();
            //int a = 20;
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                var instance = CRLManage.Instance;
                var query = instance.GetLambdaQuery();
                query.WithTrackingModel(false).WithNoLock(false);

                query.Where(b => b.Id < id2 && b.Id > id);

                var result = query.Top(1).ToString();

            }
            //Code.TestAll.TestMethod();
            sw.Stop();
            txtResult.Text = sw.ElapsedMilliseconds.ToString();
            return;
           
        }
    

        private void btnTest2_Click(object sender, EventArgs e)
        {
            var n = Convert.ToInt32(textBox2.Text);
            MappingSpeedTest.SugarQueryTest(1);
            var sw = new Stopwatch();
            //int a = 20;
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                using (var db = SugarDao.GetInstance())
                {
                    var first = db.Queryable<TestEntity>().Where(b => b.Id < id2 && b.Id > id).OrderBy(b => b.Id).Take(1).ToString2();
                }

            }
            //Code.TestAll.TestMethod();
            sw.Stop();
            txtResult.Text = sw.ElapsedMilliseconds.ToString();

        }

        int[] arry = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        int index = 0;
        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    Task.Delay(10);
                });
            }
        }
    }
}
