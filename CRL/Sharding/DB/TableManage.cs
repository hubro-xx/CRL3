using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Sharding.DB
{
    public class TableManage : CRL.BaseProvider<Table>
    {
        public static TableManage Instance
        {
            get { return new TableManage(); }
        }
        public bool Create(DataBase db, Table table,out string error)
        {
            error = "";
            table.DataBaseName = db.Name;
            if (table.IsMainTable)
            {
                table.MaxPartDataTotal = db.MaxMainDataTotal;
            }
            table.TablePartTotal = 1;
            var item = QueryItem(b => b.DataBaseName == table.DataBaseName && b.TableName == table.TableName);
            if (item != null)
            {
                error = "有重复的表" + table.TableName;
                return false;
            }
            Add(table);
            //生成分表
            var part = new TablePart();
            part.DataBaseName = table.DataBaseName;
            part.TableName = table.TableName;
            if (table.IsMainTable)
            {
                part.MainDataStartIndex = db.MainDataStartIndex;
                part.MainDataEndIndex = db.MainDataEndIndex;
            }
            else
            {
                part.MainDataStartIndex = db.MainDataStartIndex;
                part.MainDataEndIndex = db.MainDataStartIndex + table.MaxPartDataTotal-1;
            }
            part.PartName = table.TableName;
            DBExtend.InsertFromObj(part);
            return true;
        }
    }
}
