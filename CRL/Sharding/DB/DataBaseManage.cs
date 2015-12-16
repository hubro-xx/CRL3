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
        public void CleanData()
        {
            DBExtend.Delete<DataBase>(b=>b.Id>0);
            DBExtend.Delete<Table>(b => b.Id > 0);
            DBExtend.Delete<TablePart>(b => b.Id > 0);
        }
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
