using CRL.LambdaQuery.Mapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SqlSugar;
namespace TestConsole
{
    class Program
    {
         [STAThread]
        static void Main()
        {
            //var fuc = CRL.Base.CreateObjectTest<Test2>();
            //var data = new object[] { "333",1};
            //var dataContainer = new DataContainer(data);
            //var obj2 = fuc(dataContainer);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CRL.SettingConfig.GetDbAccess = (dbLocation) =>
            {
                return new CoreHelper.SqlHelper(TestConsole.DbHelper.ConnectionString);
            };

            //var s2 = new CRL.ListenTestServer(1437);
            //s2.Start();

            Application.Run(new MainForm());
        }
 
    }

}
