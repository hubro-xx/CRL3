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
    public abstract class DBAdapterBase
    {
        internal DbContext dbContext;
        protected CoreHelper.DBHelper helper;
        public DBAdapterBase(DbContext _dbContext)
        {
            dbContext = _dbContext;
            helper = dbContext.DBHelper;
        }
        /// <summary>
        /// 是否支持编译存储过程
        /// </summary>
        internal virtual bool CanCompileSP
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 根据数据库类型获取适配器
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static DBAdapterBase GetDBAdapterBase(DbContext dbContext)
        {
            DBAdapterBase db = null;
            switch (dbContext.DBHelper.CurrentDBType)
            {
                case CoreHelper.DBType.MSSQL:
                    db = new MSSQLDBAdapter(dbContext);
                    break;
                case CoreHelper.DBType.MSSQL2000:
                    db = new MSSQL2000DBAdapter(dbContext);
                    break;
                case CoreHelper.DBType.ACCESS:
                    break;
                case CoreHelper.DBType.MYSQL:
                    db = new MySQLDBAdapter(dbContext);
                    break;
                case CoreHelper.DBType.ORACLE:
                    db = new ORACLEDBAdapter(dbContext);
                    break;
                case CoreHelper.DBType.MongoDB:
                    db = new MongoDBAdapter(dbContext);
                    break;
            }
            if (db == null)
            {
                throw new CRLException("找不到对应的DBAdapte" + dbContext.DBHelper.CurrentDBType);
            }
            return db;
        }
        public abstract CoreHelper.DBType DBType { get; }
        #region 创建结构
        /// <summary>
        ///获取列类型和默认值
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public abstract string GetColumnType(CRL.Attribute.FieldAttribute info,out string defaultValue);
        /// <summary>
        /// 获取字段类型转换
        /// </summary>
        /// <returns></returns>
        public abstract System.Collections.Generic.Dictionary<Type, string> FieldMaping();
        static Dictionary<CoreHelper.DBType, Dictionary<Type, string>> _FieldMaping = new Dictionary<CoreHelper.DBType, Dictionary<Type, string>>();
        protected System.Collections.Generic.Dictionary<Type, string> GetFieldMaping()
        {
            if (!_FieldMaping.ContainsKey(dbContext.DBHelper.CurrentDBType))
            {
                _FieldMaping.Add(dbContext.DBHelper.CurrentDBType, FieldMaping());
            }
            return _FieldMaping[dbContext.DBHelper.CurrentDBType];
        }
        /// <summary>
        /// 获取字段数据库类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetDBColumnType(Type type)
        {
            var dic = GetFieldMaping();
            if (!type.FullName.StartsWith("System."))
            {
                //继承的枚举
                type = type.BaseType;
            }
            if (type.FullName.StartsWith("System.Nullable"))
            {
                //Nullable<T> 可空属性
                type = type.GenericTypeArguments[0];
            }
            if (!dic.ContainsKey(type))
            {
                throw new CRLException(string.Format("找不到对应的字段类型映射 {0} 在 {1}", type, this));
            }
            return dic[type];
        }
        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="filed"></param>
        /// <returns></returns>
        public abstract string GetColumnIndexScript(CRL.Attribute.FieldAttribute filed);
        /// <summary>
        /// 增加列
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public abstract string GetCreateColumnScript(CRL.Attribute.FieldAttribute field);
        /// <summary>
        /// 创建存储过程
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        public abstract string GetCreateSpScript(string spName, string script);
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="tableName"></param>
        public abstract void CreateTable(List<Attribute.FieldAttribute> fields, string tableName);
        #endregion

        #region SQL查询
        /// <summary>
        /// 批量插入方法
        /// </summary>
        /// <param name="details"></param>
        /// <param name="keepIdentity">否保持自增主键</param>
        public abstract void BatchInsert(System.Collections.IList details, bool keepIdentity = false);
        /// <summary>
        /// 查询表所有字段名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract string GetTableFields(string tableName);
        /// <summary>
        /// 获取UPDATE语法
        /// </summary>
        /// <param name="table"></param>
        /// <param name="setString"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual string GetUpdateSql(string table, string setString, string where)
        {
            string sql = string.Format("update {0} set {1} where {2}", KeyWordFormat(table), setString, where);
            return sql;
        }
        /// <summary>
        /// 获取删除语法
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual string GetDeleteSql(string table, string where)
        {
            string sql = string.Format("delete from {0}  where {1}", KeyWordFormat(table), where);
            return sql;
        }

        /// <summary>
        /// 插入对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract object InsertObject(CRL.IModel obj);
        /// <summary>
        /// 获取查询前几条
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="query"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public abstract string GetSelectTop(string fields, string query,string sort, int top);

        /// <summary>
        /// 获取with nolock语法
        /// </summary>
        /// <returns></returns>
        public abstract string GetWithNolockFormat(bool v);
        #endregion

        #region  系统查询
        /// <summary>
        /// 获取所有存储过程
        /// </summary>
        /// <returns></returns>
        public abstract string GetAllSPSql();
        /// <summary>
        /// 获取所有表,查询需要转为小写
        /// </summary>
        /// <returns></returns>
        public abstract string GetAllTablesSql();
        #endregion

        #region 模版
        /// <summary>
        /// 存储过程参数格式化
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public abstract string SpParameFormat(string name,string type,bool output);
        /// <summary>
        /// 关键字格式化,如SQL为 [field]
        /// </summary>
        public abstract string KeyWordFormat(string value);
        /// <summary>
        /// GROUP分页模版
        /// </summary>
        public abstract string TemplateGroupPage { get; }
        /// <summary>
        /// 查询分页模版
        /// </summary>
        public abstract string TemplatePage { get; }
        /// <summary>
        /// 存储过程模版
        /// </summary>
        public abstract string TemplateSp { get; }
        /// <summary>
        /// 语句自定义格式化处理
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public abstract string SqlFormat(string sql);
        #endregion

        /// <summary>
        /// page
        /// </summary>
        /// <param name="query"></param>
        /// <param name="fields"></param>
        /// <param name="sort"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        internal virtual CallBackDataReader GetPageData(string query, string fields, string sort, int pageSize, int pageIndex)
        {
            helper.AddParam("query_", query);
            helper.AddParam("fields_", fields);
            helper.AddParam("sort_", sort);
            helper.AddParam("pageSize_", pageSize);
            helper.AddParam("pageIndex_", pageIndex);
            helper.AddOutParam("count_",1);
            //var reader = helper.RunDataReader("sp_page");
            var reader = new CallBackDataReader(helper.RunDataReader("sp_page"), () =>
            {
                return Convert.ToInt32(helper.GetOutParam("count_"));
            });
            return reader;
        }
        #region 函数语法
        public virtual string SubstringFormat(string field, int index, int length)
        {
            return string.Format(" SUBSTRING({0},{1},{2})", field, index, length);
        }

        public virtual string StringLikeFormat(string field, string parName)
        {
            return string.Format("{0} LIKE {1}", field, parName);
        }

        public virtual string StringNotLikeFormat(string field, string parName)
        {
            return string.Format("{0} NOT LIKE {1}", field, parName);
        }

        public virtual string StringContainsFormat(string field, string parName)
        {
            return string.Format("CHARINDEX({1},{0})>0", field, parName);
        }
        public virtual string StringNotContainsFormat(string field, string parName)
        {
            return string.Format("CHARINDEX({1},{0})<=0", field, parName);
        }

        public virtual string BetweenFormat(string field, string parName, string parName2)
        {
            return string.Format("{0} between {1} and {2}", field, parName, parName2);
        }
        public virtual string NotBetweenFormat(string field, string parName, string parName2)
        {
            return string.Format("{0} not between {1} and {2}", field, parName, parName2);
        }
        public virtual string DateDiffFormat(string field, string format, string parName)
        {
            return string.Format("DateDiff({0},{1},{2})", format, field, parName);
        }

        public virtual string InFormat(string field, string parName)
        {
            return string.Format("{0} IN ({1})", field, parName);
        }
        public virtual string NotInFormat(string field, string parName)
        {
            return string.Format("{0} NOT IN ({1})", field, parName);
        }
        public abstract string CastField(string field,Type fieldType);
        public virtual string IsNotFormat(bool isNot)
        {
            return isNot ? " is not " : " is ";
        }
        public virtual string ToUpperFormat(string field)
        {
            return string.Format("upper({0})",field);
        }
        public virtual string ToLowerFormat(string field)
        {
            return string.Format("lower({0})", field);
        }
        public virtual string IsNull(string field, object value)
        {
            return string.Format("isnull({0},{1})", field, value);
        }
        #endregion

        /// <summary>
        /// 分页SQL 默认为MSSQL
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="rowOver"></param>
        /// <param name="condition"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public virtual string PageSqlFormat(string fields, string rowOver, string condition,int start,int end,string sort)
        {
            string sql = "SELECT * FROM (select {0},ROW_NUMBER() OVER ( Order by {1} ) AS RowNumber From {2}) T WHERE T.RowNumber BETWEEN {3} AND {4} order by RowNumber";
            return string.Format(sql, fields, rowOver, condition, start, end);
        }
        /// <summary>
        /// 获取关联更新语名
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="condition"></param>
        /// <param name="setValue"></param>
        /// <returns></returns>
        public virtual string GetRelationUpdateSql(string t1, string t2, string condition, string setValue)
        {
            string table = string.Format("{0} t1,{1} t2", KeyWordFormat(t1), KeyWordFormat(t2));
            string sql = string.Format("update t1 set {0} from {1} where {2}", setValue, table, condition);
            return sql;
        }
        /// <summary>
        /// 获取关联删除语句
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public virtual string GetRelationDeleteSql(string t1, string t2, string condition)
        {
            string table = string.Format("{0} t1,{1} t2", KeyWordFormat(t1), KeyWordFormat(t2));
            string sql = string.Format("delete t1 from {0} where {1}", table, condition);
            return sql;
        }
    }
}
