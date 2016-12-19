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

namespace CRL.DBAdapter
{
    internal class MySQLDBAdapter : DBAdapterBase
    {
        public MySQLDBAdapter(DbContext _dbContext)
            : base(_dbContext)
        {
        }
        #region 创建结构
        /// <summary>
        /// 创建存储过程脚本
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public override string GetCreateSpScript(string spName, string script)
        {
            throw new NotSupportedException("MySql不支持动态创建存储过程");
            string template = string.Format(@"
drop procedure if exists {0};
EXECUTE  ' {1} ';
", spName, script);
            return template;
        }

        /// <summary>
        /// 获取字段类型映射
        /// </summary>
        /// <returns></returns>
        public override Dictionary<Type, string> FieldMaping()
        {
            Dictionary<Type, string> dic = new Dictionary<Type, string>();
            //字段类型对应
            dic.Add(typeof(System.String), "varchar({0})");
            dic.Add(typeof(System.Decimal), "decimal(18, 2)");
            dic.Add(typeof(System.Double), "float");
            dic.Add(typeof(System.Single), "real");
            dic.Add(typeof(System.Boolean), "tinyint(1)");
            dic.Add(typeof(System.Int32), "int");
            dic.Add(typeof(System.Int16), "SMALLINT");
            dic.Add(typeof(System.Enum), "int");
            dic.Add(typeof(System.Byte), "SMALLINT");
            dic.Add(typeof(System.DateTime), "datetime");
            dic.Add(typeof(System.UInt16), "SMALLINT");
            dic.Add(typeof(System.Object), "varchar(30)");
            dic.Add(typeof(System.Byte[]), "varbinary({0})");
            dic.Add(typeof(System.Guid), "varchar(50)");
            return dic;
        }
        /// <summary>
        /// 获取列类型和默认值
        /// </summary>
        /// <param name="info"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public override string GetColumnType(Attribute.FieldAttribute info, out string defaultValue)
        {
            Type propertyType = info.PropertyType;
            //Dictionary<Type, string> dic = GetFieldMaping();
            defaultValue = info.DefaultValue;

            //int默认值
            if (string.IsNullOrEmpty(defaultValue))
            {
                if (!info.IsPrimaryKey && propertyType == typeof(System.Int32))
                {
                    defaultValue = "0";
                }
                //datetime默认值
                if (propertyType == typeof(System.DateTime))
                {
                    defaultValue = " CURRENT_TIMESTAMP";
                }
            }
            string columnType;

            columnType = GetDBColumnType(propertyType);
            //超过3000设为ntext
            if (propertyType == typeof(System.String) && info.Length > 3000)
            {
                columnType = "varchar(max)";
            }
            if (info.Length > 0)
            {
                columnType = string.Format(columnType, info.Length);
            }
            if (info.IsPrimaryKey)
            {
                if (info.KeepIdentity)
                {
                    columnType = " " + columnType + " primary key";
                }
                else
                {
                    //todo 只有数值型才能自增
                    columnType = " " + columnType + " primary key auto_increment";
                }
            }

            if (!string.IsNullOrEmpty(info.ColumnType))
            {
                columnType = info.ColumnType;
            }
            return columnType;
        }

        /// <summary>
        /// 创建字段脚本
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public override string GetCreateColumnScript(Attribute.FieldAttribute field)
        {
            string str = string.Format("alter table `{0}` add {1} {2}", field.TableName, field.MapingName, field.ColumnType);
            if (!string.IsNullOrEmpty(field.DefaultValue))
            {
                str += string.Format(" default '{0}' ", field.DefaultValue);
            }
            if (field.NotNull)
            {
                str += " not null";
            }
            return str;
        }

        /// <summary>
        /// 创建索引脚本
        /// </summary>
        /// <param name="filed"></param>
        /// <returns></returns>
        public override string GetColumnIndexScript(Attribute.FieldAttribute filed)
        {
            string indexScript = string.Format("ALTER TABLE `{0}` ADD {2} ({1}) ", filed.TableName, filed.MapingName, filed.FieldIndexType == Attribute.FieldIndexType.非聚集唯一 ? "UNIQUE" : "INDEX index_name");
            return indexScript;
        }

        /// <summary>
        /// 创建表脚本
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override void CreateTable(List<Attribute.FieldAttribute> fields, string tableName)
        {
            var defaultValues = new List<string>();
            string script = string.Format("create table {0}(\r\n", tableName);
            List<string> list2 = new List<string>();
            foreach (Attribute.FieldAttribute item in fields)
            {
                string nullStr = item.NotNull ? "NOT NULL" : "";
                var columnType = GetDBColumnType(item.PropertyType);

                string str = string.Format("{0} {1} {2} ", item.MapingName, item.ColumnType, nullStr);

                list2.Add(str);
                
            }
            script += string.Join(",\r\n", list2.ToArray());
            script += ") ";
            helper.Execute(script);
            foreach (string s in defaultValues)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    helper.Execute(s);
                }
            }
        }
        #endregion
        public override CoreHelper.DBType DBType
        {
            get { return CoreHelper.DBType.MYSQL; }
        }
        #region SQL查询
        public override string GetTableFields(string tableName)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 批量插入,mysql不支持批量插入
        /// </summary>
        /// <param name="details"></param>
        /// <param name="keepIdentity"></param>
        public override void BatchInsert(System.Collections.IList details, bool keepIdentity = false) 
        {
            foreach(var item in details)
            {
                helper.ClearParams();
                InsertObject(item as IModel);
            }
            
        }

        /// <summary>
        /// 获取插入语法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override object InsertObject(IModel obj)
        {
            Type type = obj.GetType();
            string table = TypeCache.GetTableName(type, dbContext);
            var typeArry = TypeCache.GetProperties(type, true).Values;
            Attribute.FieldAttribute primaryKey = null;
            string sql = string.Format("insert into `{0}`(", table);
            string sql1 = "";
            string sql2 = "";
            foreach (Attribute.FieldAttribute info in typeArry)
            {
                if (info.FieldType != Attribute.FieldType.数据库字段)
                {
                    continue;
                }
                string name = info.MapingName;
                if (info.IsPrimaryKey)
                {
                    primaryKey = info;
                }
                if (info.IsPrimaryKey && !info.KeepIdentity)
                {
                    continue;
                }
                //if (!string.IsNullOrEmpty(info.VirtualField))
                //{
                //    continue;
                //}
                object value = info.GetValue(obj);
                if (info.PropertyType.FullName.StartsWith("System.Nullable"))//Nullable<T>类型为空值不插入
                {
                    if (value == null)
                    {
                        continue;
                    }
                }
                value = ObjectConvert.CheckNullValue(value, info.PropertyType);
                sql1 += string.Format("{0},", info.MapingName);
                sql2 += string.Format("?{0},", name);
                helper.AddParam(name, value);
            }
            sql1 = sql1.Substring(0, sql1.Length - 1);
            sql2 = sql2.Substring(0, sql2.Length - 1);
            sql += sql1 + ") values( " + sql2 + ") ; ";
            sql = SqlFormat(sql);
            if (primaryKey.KeepIdentity)
            {
                helper.Execute(sql);
                return primaryKey.GetValue(obj);
            }
            else
            {
                sql += "SELECT LAST_INSERT_ID();";
                return helper.ExecScalar(sql);
            }
        }
        /// <summary>
        /// 获取 with(nolock)
        /// </summary>
        /// <returns></returns>
        public override string GetWithNolockFormat(bool v)
        {
            return "";
        }
        /// <summary>
        /// 获取前几条语句
        /// </summary>
        /// <param name="fields">id,name</param>
        /// <param name="query">from table where 1=1</param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override string GetSelectTop(string fields, string query,string sort, int top)
        {
            string sql = string.Format("select {1} {2} {3} {0}", top == 0 ? "" : " LIMIT 0, " + top, fields, query, sort);
            return sql;
        }
        #endregion

        #region 系统查询
        public override string GetAllTablesSql()
        {
            return "select lower(table_name),1 from information_schema.tables where table_schema='" + helper.DatabaseName + "' ";
        }
        public override string GetAllSPSql()
        {
            return "select `name`,1 from mysql.proc where db = '" + helper.DatabaseName + "' and `type` = 'PROCEDURE' ";
        }
        #endregion

        #region 模版
        public override string SpParameFormat(string name, string type, bool output)
        {
            string str = "";
            if (!output)
            {
                str = "in {0} {1},";
            }
            else
            {
                str = "out {0} {1},";
            }
            return string.Format(str, name, type);
        }

        public override string KeyWordFormat(string value)
        {
            return string.Format("`{0}`", value);
        }
        public override string TemplateGroupPage
        {
            get
            {
                throw new NotSupportedException("MySql不支持动态创建存储过程");
                string str = @"
CREATE PROCEDURE {name}
( 
	{parame}
) 

BEGIN
 if pageSize<=1 then 
  set pageSize=20;
 end if;
 if pageIndex < 1 then 
  set pageIndex = 1; 
 end if;
 
 set @strsql = concat('select {fields} from {sql} order by {sort} limit ',_pageIndex*_pageSize-_pageSize,',',_pageSize); 
 prepare stmtsql from @strsql; 
 execute stmtsql; 
 deallocate prepare stmtsql;
 set @strsqlcount='select count(1) as count from {sql}';
 prepare stmtsqlcount from @strsqlcount; 
 execute stmtsqlcount; 
 deallocate prepare stmtsqlcount; 
END
";
                return str;
            }
        }

        public override string TemplatePage
        {
            get
            {
                throw new NotSupportedException("MySql不支持动态创建存储过程");
                string str = @"
CREATE PROCEDURE {name}
( 
	{parame}
) 

BEGIN
 if pageSize<=1 then 
  set pageSize=20;
 end if;
 if pageIndex < 1 then 
  set pageIndex = 1; 
 end if;
 
 set @strsql = concat('select {fields} from {sql} order by {sort} limit ',_pageIndex*_pageSize-_pageSize,',',_pageSize); 
 prepare stmtsql from @strsql; 
 execute stmtsql; 
 deallocate prepare stmtsql;
 set @strsqlcount='select count(1) as count from {sql}';
 prepare stmtsqlcount from @strsqlcount; 
 execute stmtsqlcount; 
 deallocate prepare stmtsqlcount; 
END

";
                return str;
            }
        }

        public override string TemplateSp
        {
            get
            {
                throw new NotSupportedException("MySql不支持动态创建存储过程");
                string str = @"
CREATE PROCEDURE {name}
({parame})
begin
	{sql};
end
";
                return str;
            }
        }
        public override string SqlFormat(string sql)
        {
            return System.Text.RegularExpressions.Regex.Replace(sql, @"@(\w+)", "?$1");
        }
        #endregion

        public override string SubstringFormat(string field, int index, int length)
        {
            throw new NotImplementedException();
        }

        public override string StringLikeFormat(string field, string parName)
        {
            throw new NotImplementedException();
        }

        public override string StringNotLikeFormat(string field, string parName)
        {
            throw new NotImplementedException();
        }

        public override string StringContainsFormat(string field, string parName)
        {
            throw new NotImplementedException();
        }
        public virtual string StringNotContainsFormat(string field, string parName)
        {
            return string.Format("CHARINDEX({1},{0})<=0", field, parName);
        }
        public override string BetweenFormat(string field, string parName, string parName2)
        {
            throw new NotImplementedException();
        }
        public virtual string NotBetweenFormat(string field, string parName, string parName2)
        {
            return string.Format("{0} not between {1} and {2}", field, parName, parName2);
        }
        public override string DateDiffFormat(string field, string format, string parName)
        {
            throw new NotImplementedException();
        }

        public override string InFormat(string field, string parName)
        {
            throw new NotImplementedException();
        }

        public override string NotInFormat(string field, string parName)
        {
            throw new NotImplementedException();
        }
        public override string PageSqlFormat(string fields, string rowOver, string condition, int start, int end, string sort)
        {
            string sql = "SELECT {0} FROM {1} order by {4} limit {2},{3} ";
            return string.Format(sql, fields, condition, start, end, sort);
        }
        public override string GetRelationUpdateSql(string t1, string t2, string condition, string setValue)
        {
            string sql = string.Format("update {0} t1, {1} t2 set {2} where {3}", KeyWordFormat(t1)
                  , KeyWordFormat(t2), setValue, condition);
            return sql;
        }
        public override string CastField(string field, Type fieldType)
        {
            throw new NotImplementedException();
        }
    }
}
