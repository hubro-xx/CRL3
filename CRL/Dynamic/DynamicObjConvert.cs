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
        static dynamic getDataRow(DataRow dr)
        {
            dynamic obj = new System.Dynamic.ExpandoObject();
            var dict = obj as IDictionary<string, object>;
            foreach (DataColumn col in dr.Table.Columns)
            {
                dict.Add(col.ColumnName, dr[col.ColumnName]);
            }
            return obj;
        }
        public static IEnumerable<dynamic> _DataTableToDynamic(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                var d = getDataRow(row);
                yield return d;
            }
        }
        static dynamic getDataRow(List<string> columns,object[] values)
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

            while (reader.Read())
            {
                object[] values = new object[columns.Count];
                reader.GetValues(values);
                var d = getDataRow(columns, values);
                list.Add(d);
            }
            reader.Close();
            runTime = (DateTime.Now - time).TotalMilliseconds;
            return list;
        }
        /// <summary>
        /// 返回匿名类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="resultSelector"></param>
        /// <param name="runTime"></param>
        /// <returns></returns>
        public static List<TResult> DataReaderToDynamic<T, TResult>(System.Data.Common.DbDataReader reader, Expression<Func<T, TResult>> resultSelector, out double runTime) where T : IModel, new()
        {
            var time = DateTime.Now;
            List<TResult> list = new List<TResult>();
            var typeArry = TypeCache.GetProperties(typeof(T), true).Values;
            //var columns = new Dictionary<int, string>();
            //for (int i = 0; i < reader.FieldCount; i++)
            //{
            //    columns.Add(i, reader.GetName(i).ToLower());
            //}
            var columns = new Dictionary<string, int>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i).ToLower(), i);
            }
            var leftColumns = new Dictionary<string, int>(columns);
            var reflection = ReflectionHelper.GetInfo<T>();
            var actions = new List<CRL.ObjectConvert.ActionItem<T>>();
            var first = true;
            //var objOrigin = new T();
            while (reader.Read())
            {
                object objInstance = reflection.CreateObjectInstance();
        
                object[] values = new object[columns.Count];
                reader.GetValues(values);
                //var dic = new Dictionary<string, object>();
                //for (int i = 0; i < columns.Count; i++)
                //{
                //    var name = columns[i];
                //    dic.Add(name.ToLower(), values[i]);
                //}
                var detailItem = ObjectConvert.DataReaderToObj<T>(columns, values, reflection, true, objInstance, typeArry, actions, first, leftColumns) as T;
                var result = resultSelector.Compile()(detailItem);
                list.Add(result);
                first = false;
            }
            reader.Close();
            runTime = (DateTime.Now - time).TotalMilliseconds;
            return list;
        }
    }
}
