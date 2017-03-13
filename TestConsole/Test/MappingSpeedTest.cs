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
using System.Configuration;
using System.Linq.Expressions;

namespace TestConsole
{
    class DbHelper
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
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
        public static void LoognQueryTest(int takeCount)
        {
            using (var db = new SqlConnection(DbHelper.ConnectionString))
            {
                string sql = string.Format("select top {0} * from TestEntity" + (takeCount == 1 ? " where id<" + id2 + " and id>" + id : ""), takeCount.ToString());
                var list = db.SelectFmt<TestEntityCRL>(sql);
            }
        }
        static int id = 20;
        static int id2 = 30;
        static string name = "123";
        public static void CRLQueryTest(int takeCount)
        {
            var instance = CRLManage.Instance;
            var query = instance.GetLambdaQuery();
            query.WithTrackingModel(false).WithNoLock(false);
            if (takeCount == 1)
            {
                query.Where(b => b.Id < id2 && b.Id > id);
            }
            var result = query.Top(takeCount).ToList();

        }
        public static void CRLSQLQueryTest(int takeCount)
        {
            var instance = CRLManage.Instance;

            string sql = string.Format("select top {0} * from TestEntity" + (takeCount == 1 ? " where id<" + id2 + " and id>" + id : ""), takeCount.ToString());
            instance.Test(sql);
        }

        public static void SugarQueryTest(int takeCount)
        {
            using (var db = SugarDao.GetInstance())
            {
                if (takeCount == 1)
                {
                    var first = db.Queryable<TestEntity>().Where(b => b.Id < id2 && b.Id > id).OrderBy(b => b.Id).Take(takeCount).ToList();
                }
                else
                {
                    var first = db.Queryable<TestEntity>().OrderBy(b => b.Id).Take(takeCount).ToList();
                }
            }
        }

        public static void ChloeQueryTest(int takeCount)
        {
            using (MsSqlContext context = new MsSqlContext(DbHelper.ConnectionString))
            {
                if (takeCount == 1)
                {
                    var list = context.Query<TestEntity>().Where(b => b.Id < id2 && b.Id > id).Take(takeCount).ToList();
                }
                else
                {
                    var list = context.Query<TestEntity>().Take(takeCount).ToList();
                }
            }
        }


        public static void DapperQueryTest(int takeCount)
        {
            using (var conn = DbHelper.CreateConnection())
            {
                string sql = string.Format("select top {0} * from TestEntity" + (takeCount == 1 ? " where id<" + id2 + " and id>" + id : ""), takeCount.ToString());
                var list = conn.Query<TestEntity>(sql).ToList();
            }
        }

        public static void EFLinqQueryTest(int takeCount)
        {
            using (EFContext efContext = new EFContext())
            {
                if (takeCount == 1)
                {
                    var list = efContext.TestEntity.AsNoTracking().Where(b => b.Id < id2 && b.Id > id).Take(takeCount).ToList();
                }
                else
                {
                    var list = efContext.TestEntity.AsNoTracking().Take(takeCount).ToList();
                }
            }
        }
        public static void EFSqlQueryTest(int takeCount)
        {
            using (EFContext efContext = new EFContext())
            {
                string sql = string.Format("select top {0} * from TestEntity" + (takeCount == 1 ? " where id<" + id2 + " and id>" + id : ""), takeCount.ToString());
                var list = efContext.Database.SqlQuery<TestEntity>(sql).ToList();
            }
        }
        public static void LinqToDBQueryTest(int takeCount)
        {
            using (var db = new LinqToDBContext())
            {
                if (takeCount == 1)
                {
                    var query = (from p in db.TestEntitys
                                 where p.Id < id2 && p.Id > id
                                 select p).Take(takeCount);
                    var result = query.ToList();
                }
                else
                {
                    var query = (from p in db.TestEntitys
                                 select p).Take(takeCount);
                    var result = query.ToList();
                }
            }
        }
        
    }
}