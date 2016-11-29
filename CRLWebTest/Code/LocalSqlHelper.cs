/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace WebTest.Code
{
    public class LocalSqlHelper
    {

        static CoreHelper.DBHelper CreateDbHelper(string name)
        {
            string connString;
            //mssql
            //更改DBConnection目录内数据连接文件
            connString = CoreHelper.CustomSetting.GetConnectionString(name);
            return new CoreHelper.SqlHelper(connString);

            ////mysql
            //connString = "Server=127.0.0.1;Port=3306;Stmt=;Database=testDB; User=root;Password=;";
            //return new CoreHelper.MySqlHelper(connString);

            //oracle
            //connString = "Data Source={0};User ID={1};Password={2};Integrated Security=no;";
            //connString = string.Format(connString, "orcl", "SCOTT", "a123");
            //return new CoreHelper.OracleHelper(connString);
        }
        public static CoreHelper.DBHelper TestConnection
        {
            get
            {
                return CreateDbHelper("Default");
            }
        }
        public static CoreHelper.DBHelper MongoDB
        {
            get
            {
                var connString = CoreHelper.CustomSetting.GetConnectionString("mongodb");
                //只是用来传连接串
                return new CoreHelper.MongoDBHelper(connString, "test2");
            }
        }
        public static CoreHelper.DBHelper TestConnection2
        {
            get
            {
                return CreateDbHelper("db2");
            }
        }
    }
}
