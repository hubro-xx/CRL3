/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CRL.Dynamic
{
    internal class DynamicObjConvert
    {

        static dynamic getRow(List<string> columns,object[] values)
        {
            dynamic obj = new System.Dynamic.ExpandoObject();
            var dict = obj as IDictionary<string, object>;
            for (int i = 0; i < values.Count(); i++)
            {
                string columnName = columns[i];
                object value = values[i];
                dict.Add(columnName, value);
            }
            return obj;
        }
        public static List<dynamic> DataReaderToDynamic(System.Data.Common.DbDataReader reader, out double runTime)
        {
            var time = DateTime.Now;
            List<dynamic> list = new List<dynamic>();
            var columns = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }
            try
            {
                #region while
                while (reader.Read())
                {
                    object[] values = new object[columns.Count];
                    reader.GetValues(values);
                    var d = getRow(columns, values);
                    list.Add(d);
                }
                #endregion
            }
            catch(Exception ero)
            {
                reader.Close();
                throw new CRLException("读取数据时发生错误:" + ero.Message);
            }
            reader.Close();
            runTime = (DateTime.Now - time).TotalMilliseconds;
            return list;
        }
    }
}
