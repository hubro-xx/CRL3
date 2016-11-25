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
    public class TablePartManage : CRL.BaseProvider<TablePart>
    {
        public static TablePartManage Instance
        {
            get { return new TablePartManage(); }
        }
        /// <summary>
        /// 创建表分区配置
        /// </summary>
        /// <param name="table"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Create(Table table,  out string error)
        {
            error = "";
            var query = GetLambdaQuery();
            query.Where(b => b.DataBaseName == table.DataBaseName && b.TableName == table.TableName);
            query.Top(1);
            query.OrderBy(b => b.MainDataEndIndex, true);
            var part1 = query.ToList().FirstOrDefault();
            var start = part1.MainDataEndIndex += 1;
            var end = start + table.MaxPartDataTotal - 1;
            TablePart part = new TablePart() { DataBaseName = table.DataBaseName, TableName = table.TableName, MainDataStartIndex = start, MainDataEndIndex = end, PartIndex = part1.PartIndex + 1 };
            if (part.PartIndex == 0)
            {
                part.PartName = table.TableName;
            }
            else
            {
                part.PartName = string.Format("{0}_{1}", table.TableName, part.PartIndex);
            }
            Add(part);
            table.TablePartTotal = part.PartIndex + 1;
            DBExtend.Update(table);
            return true;
        }
        //public int UpdateIndex(DataBase dataBase, TablePart tablePart)
        //{
        //    string error;
        //    int index = dataBase.MainDataEndIndex;
        //    var a = PackageTrans(dbHelper, (out string ero) =>
        //    {
        //        ero = "";
        //        //超过最大值
        //        if (index >= tablePart.MaxPartDataTotal)
        //        {
        //        }
        //        else
        //        {
        //            index += 1;
        //            dataBase.MainDataEndIndex = index;
        //            //tablePart.DataEndIndex = index;
        //            DBHelper.Update(dataBase);
        //            DBHelper.Update(tablePart);
        //        }
        //        return true;
        //    }, out error);
        //    if (!a)
        //    {
        //        index -= 1;
        //        dataBase.MainDataEndIndex = index;
        //        //tablePart.DataEndIndex = index;
        //        return 0;
        //    }
        //    return index;
        //}
    }
}
