/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL
{
    public class DBExtendFactory
    {
        public static AbsDBExtend CreateDBExtend(DbContext _dbContext)
        {
            if (_dbContext.DBHelper.CurrentDBType == CoreHelper.DBType.MongoDB)
            {
                return new DBExtend.MongoDB.MongoDB(_dbContext);
            }
            return new DBExtend.RelationDB.DBExtend(_dbContext);
        }
    }
}
