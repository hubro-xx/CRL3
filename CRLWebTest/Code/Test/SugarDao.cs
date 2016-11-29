
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChloePerformanceTest
{
    /// <summary>
    /// SqlSugar
    /// </summary>
    public class SugarDao
    {
        //禁止实例化
        private SugarDao()
        {

        }
        public static string ConnectionString
        {
            get
            {
                string reval = DbHelper.ConnectionString;
                return reval;
            }
        }
        public static SqlSugarClient GetInstance()
        {
            var db = new SqlSugarClient(ConnectionString);
            //db.IsEnableLogEvent = true;//启用日志事件
            //db.LogEventStarting = (sql, par) => { Console.WriteLine(sql + " " + par + "\r\n"); };
            return db;
        }
    }
}
