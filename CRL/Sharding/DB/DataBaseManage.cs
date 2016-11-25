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

namespace CRL.Sharding.DB
{
    public class DataBaseManage : CRL.BaseProvider<DataBase>
    {
        public static DataBaseManage Instance
        {
            get { return new DataBaseManage(); }
        }
        /// <summary>
        /// 清除配置
        /// </summary>
        public void CleanData()
        {
            DBExtend.Delete<DataBase>(b=>b.Id>0);
            DBExtend.Delete<Table>(b => b.Id > 0);
            DBExtend.Delete<TablePart>(b => b.Id > 0);
        }
        /// <summary>
        /// 创建库配置
        /// 多次调用递增处理
        /// </summary>
        /// <param name="item"></param>
        public void Create(DataBase item)
        {
            var db = QueryItem(b => b.Id > 0, true);
            if (db == null)
            {
                item.Name = "db1";
                item.MainDataStartIndex = 1;
                item.MainDataEndIndex = item.MaxMainDataTotal;
            }
            else
            {
                item.MainDataStartIndex = db.MainDataEndIndex + 1;
                item.MainDataEndIndex = item.MainDataStartIndex + db.MaxMainDataTotal - 1;
                item.MaxMainDataTotal = db.MaxMainDataTotal;
                item.Name = "db" + (db.Id + 1);
            }
            Add(item);
        }
    }
}
