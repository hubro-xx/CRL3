using CRL.LambdaQuery.Mapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Application.Run(new MainForm());
        }
 
    }
    public class Test2 : CRL.IModel
    {
        public string Name
        {
            get;
            set;
        }
        public string Id
        {
            get;
            set;
        }
        public int? F_Int32 { get; set; }
    }
}
