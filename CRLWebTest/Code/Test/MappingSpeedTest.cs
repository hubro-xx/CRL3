using Chloe.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using Loogn.OrmLite;
using SqlSugar;
using System.Data.Common;

namespace ChloePerformanceTest
{
    class DbHelper
    {
        public static string ConnectionString
        {
            get
            {
                return CoreHelper.CustomSetting.GetConnectionString("default");
            }
        }
        public static DbConnection CreateConnection()
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            return conn;
        }
    }
    class MappingSpeedTest
    {
        //static CRLManage instance = new CRLManage();

        public static void LoognQueryTest(int takeCount)
        {
            using (var db = new SqlConnection(DbHelper.ConnectionString))
            {
                var list = db.SelectFmt<TestEntityCRL>("select top {0} * from TestEntity", takeCount.ToString());
            }
        }
        public static void CRLQueryTest(int takeCount)
        {
            var instance = CRLManage.Instance;
            var query = instance.GetLambdaQuery();
            query.WithTrackingModel(false).WithNoLock(false);
            var result = query.Top(takeCount).ToList();
        }

        public static void SugarQueryTest(int takeCount)
        {
            using (var db = SugarDao.GetInstance())
            {
                var first = db.Queryable<TestEntity>().Where(b => b.Id > 0).OrderBy(b => b.Id).Take(takeCount).ToList();
            }
        }

        public static void ChloeQueryTest(int takeCount)
        {
            using (MsSqlContext context = new MsSqlContext(DbHelper.ConnectionString))
            {
                var list = context.Query<TestEntity>().Take(takeCount).ToList();
            }
        }


        public static void DapperQueryTest(int takeCount)
        {
            using (var conn = DbHelper.CreateConnection())
            {
                var list = conn.Query<TestEntity>(string.Format("select top {0} * from TestEntity", takeCount.ToString())).ToList();
            }
        }

        public static void EFLinqQueryTest(int takeCount)
        {
            using (EFContext efContext = new EFContext())
            {
                var list = efContext.TestEntity.AsNoTracking().Take(takeCount).ToList();
            }
        }
        public static void EFSqlQueryTest(int takeCount)
        {
            using (EFContext efContext = new EFContext())
            {
                var list = efContext.Database.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity", takeCount.ToString())).ToList();
            }
        }
        
    }
}