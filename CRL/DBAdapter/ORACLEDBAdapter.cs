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
using System.Data;

namespace CRL.DBAdapter
{
    internal class ORACLEDBAdapter : DBAdapterBase
    {
        public ORACLEDBAdapter(DbContext _dbContext)
            : base(_dbContext)
        {
        }
        internal override bool CanCompileSP
        {
            get
            {
                return false;
            }
        }
        public override CoreHelper.DBType DBType
        {
            get { return CoreHelper.DBType.ORACLE; }
        }
        #region 创建结构
        /// <summary>
        /// 创建存储过程脚本
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public override string GetCreateSpScript(string spName, string script)
        {
            throw new NotSupportedException("ORACLE不支持动态创建存储过程");
            string template = string.Format(@"EXECUTE  ' {1} ';", spName, script);
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
            dic.Add(typeof(System.String), "VARCHAR2({0})");
            dic.Add(typeof(System.Decimal), "NUMBER");
            dic.Add(typeof(System.Double), "DOUBLE PRECISION");
            dic.Add(typeof(System.Single), "FLOAT(24)");
            dic.Add(typeof(System.Boolean), "INTEGER");
            dic.Add(typeof(System.Int32), "INTEGER");
            dic.Add(typeof(System.Int16), "INTEGER");
            dic.Add(typeof(System.Enum), "INTEGER");
            dic.Add(typeof(System.Byte), "INTEGER");
            dic.Add(typeof(System.DateTime), "TIMESTAMP");
            dic.Add(typeof(System.UInt16), "INTEGER");
            dic.Add(typeof(System.Object), "NARCHAR2(30)");
            dic.Add(typeof(System.Byte[]), "BLOB");
            dic.Add(typeof(System.Guid), "VARCHAR2(50)");
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
                    defaultValue = "TIMESTAMP";
                }
            }
            string columnType;
            columnType = GetDBColumnType(propertyType);
            //超过3000设为ntext
            if (propertyType == typeof(System.String) && info.Length > 3000)
            {
                columnType = "CLOB";
            }
            if (info.Length > 0)
            {
                columnType = string.Format(columnType, info.Length);
            }
            //if (info.IsPrimaryKey)
            //{
            //    columnType = "NUMBER(4) Not Null Primary Key";
            //}
            if (info.IsPrimaryKey)
            {
                columnType = " " + columnType + " Primary Key ";
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
            string str = string.Format("alter table {0} add {1} {2};", field.TableName, field.MapingName, field.ColumnType);
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
            string indexName = string.Format("pk_{0}_{1}", filed.TableName, filed.MapingName);
            string indexScript = string.Format("create {3} index {0} on {1}({2}); ", indexName, filed.TableName, filed.MapingName, filed.FieldIndexType == Attribute.FieldIndexType.非聚集唯一 ? "UNIQUE" : "");
            return indexScript;
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="tableName"></param>
        public override void CreateTable(List<Attribute.FieldAttribute> fields, string tableName)
        {
            var lines = new List<string>();
            //tableName = tableName.ToUpper();
            string script = string.Format("create table {0}(\r\n", tableName);
            List<string> list2 = new List<string>();
            string primaryKey = "id";
            foreach (Attribute.FieldAttribute item in fields)
            {
                if (item.IsPrimaryKey)
                {
                    primaryKey = item.MapingName;
                }
                var columnType = GetDBColumnType(item.PropertyType);
                string nullStr = item.NotNull ? "NOT NULL" : "";
                string str = string.Format("{0} {1} {2} ", item.MapingName, item.ColumnType, nullStr);

                list2.Add(str);

            }
            script += string.Join(",\r\n", list2.ToArray());
            script += ")";
            string sequenceName = string.Format("{0}_sequence", tableName);
            string triggerName = string.Format("{0}_trigge", tableName);
            string sequenceScript = string.Format("Create Sequence {0} MINVALUE 1  MAXVALUE 99999 INCREMENT BY 1 START WITH 1 NOCACHE CYCLE", sequenceName);
            string triggerScript = string.Format(@"
create or replace trigger {0}
  before insert on {1}   
  for each row
declare
  nextid number;
begin
  IF :new.{3} IS NULL or :new.{3}=0 THEN
    select {2}.nextval 
    into nextid
    from sys.dual;
    :new.{3}:=nextid;
  end if;
end ;", triggerName, tableName, sequenceName, primaryKey);
            lines.Add(sequenceScript);
            //defaultValues.Add(triggerScript); 暂不用触发器,不能编译成功
            //script += script2;
            helper.SetParam("script", script);
            helper.Run("sp_ExecuteScript");
            //helper.SetParam("script", sequenceScript);
            //helper.Run("sp_ExecuteScript");
            //helper.SetParam("script", triggerScript);
            //helper.Run("sp_ExecuteScript");

            foreach (string s in lines)
            {
                try
                {
                    helper.Execute(s);
                }
                catch (Exception ero) { };
            }
        }
        #endregion

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
            foreach (var item in details)
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
            string sql = string.Format("insert into {0}(", table);
            string sql1 = "";
            string sql2 = "";

            string sequenceName = string.Format("{0}_sequence", table);
            var sqlGetIndex = string.Format("select {0}.nextval from dual", sequenceName);//oracle不能同时执行多条语句
            int id = Convert.ToInt32(helper.ExecScalar(sqlGetIndex));
            foreach (Attribute.FieldAttribute info in typeArry)
            {
                if (info.FieldType != Attribute.FieldType.数据库字段)
                {
                    continue;
                }
                string name = info.MapingName;
                if (info.IsPrimaryKey && !info.KeepIdentity)
                {
                    //continue;//手动插入ID
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
                sql2 += string.Format("@{0},", info.MapingName);
                helper.AddParam(info.MapingName, value);
            }
            sql1 = sql1.Substring(0, sql1.Length - 1);
            sql2 = sql2.Substring(0, sql2.Length - 1);
            sql += sql1 + ") values( " + sql2 + ")";
            sql = SqlFormat(sql);
            var primaryKey = TypeCache.GetTable(obj.GetType()).PrimaryKey;
            helper.SetParam(primaryKey.MapingName, id);
            helper.Execute(sql);
            //var helper2 = helper as CoreHelper.OracleHelper;
            //int id = helper2.Insert(sql,sequenceName);
            return id;
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
            if (!query.ToLower().Contains("where"))
            {
                query += " where 1=1 ";
            }
            string sql = string.Format("select {0} {1} {2} {3}", fields, query, top == 0 ? "" : " and ROWNUM<=" + top,sort);
            return sql;
        }
        #endregion

        #region 系统查询
        public override string GetAllTablesSql()
        {
            return "SELECT lower(table_name),1 FROM user_TABLES";
        }
        public override string GetAllSPSql()
        {
            return "select object_name,1 from user_objects where object_type='PROCEDURE'";
        }
        #endregion

        #region 模版
        public override string SpParameFormat(string name, string type, bool output)
        {
            string str = "";
            if (!output)
            {
                str = "{0} in {1},";
            }
            else
            {
                str = "{0} out {1},";
            }
            return string.Format(str, name, type);
        }

        public override string KeyWordFormat(string value)
        {
            string keyWords = " ACCESS  ADD  ALL  ALTER  AND  ANY  AS  ASC  AUDIT  BETWEEN  BY  CHAR CHECK  CLUSTER  COLUMN  COMMENT  COMPRESS  CONNECT  CREATE  CURRENT DATE  DECIMAL  DEFAULT  DELETE  DESC  DISTINCT  DROP  ELSE  EXCLUSIVE EXISTS  FILE  FLOAT FOR  FROM  GRANT  GROUP  HAVING  IDENTIFIED IMMEDIATE  IN  INCREMENT  INDEX  INITIAL  INSERT  INTEGER  INTERSECT INTO  IS  LEVEL  LIKE  LOCK  LONG  MAXEXTENTS  MINUS  MLSLABEL  MODE MODIFY  NOAUDIT  NOCOMPRESS  NOT  NOWAIT  NULL  NUMBER  OF  OFFLINE ON  ONLINE  OPTION  OR  ORDER P CTFREE PRIOR PRIVILEGES PUBLIC RAW RENAME RESOURCE REVOKE ROW ROWID ROWNUM ROWS SELECT SESSION SET SHARE SIZE SMALLINT START SUCCESSFUL SYNONYM SYSDATE TABLE THEN TO TRIGGER UID UNION UNIQUE UPDATE USER VALIDATE VALUES VARCHAR VARCHAR2 VIEW WHENEVER WHERE WITH ";
            //keyword 
            if (keyWords.Contains(" " + value.ToUpper() + " "))
            {
                return value + "_";
            }
            return value;
        }
        public override string TemplateGroupPage
        {
            get
            {
                throw new NotSupportedException("ORACLE不支持动态创建存储过程");
                string str = @"
create or replace PROCEDURE {name}
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
                throw new NotSupportedException("ORACLE不支持动态创建存储过程");
                string str = @"
create or replace PROCEDURE {name}
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
                throw new NotSupportedException("ORACLE不支持动态创建存储过程");
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
            return System.Text.RegularExpressions.Regex.Replace(sql, @"@(\w+)", ":$1");
        }
        #endregion

        internal override CallBackDataReader GetPageData( string query, string fields, string sort, int pageSize, int pageIndex)
        {
            helper.AddParam("query_", query);
            helper.AddParam("fields_", fields);
            helper.AddParam("sort_", sort);
            helper.AddParam("pageSize_", pageSize);
            helper.AddParam("pageIndex_", pageIndex);
            helper.AddOutParam("count_");
            helper.AddOutParam("v_Cursor");
            var reader = new CallBackDataReader(helper.RunDataReader("sp_page"), () =>
            {
                return Convert.ToInt32(helper.GetOutParam("count_"));
            });
            return reader;
        }



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
        public override string GetRelationUpdateSql(string t1, string t2, string condition, string setValue)
        {
            throw new NotImplementedException();
        }
        public override string CastField(string field, Type fieldType)
        {
            throw new NotImplementedException();
        }
    }
}
