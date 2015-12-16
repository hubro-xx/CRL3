using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CRL
{
    /// <summary>
    /// 基类,包含Id, AddTime字段
    /// </summary>
    [Serializable]
    public abstract class IModelBase : IModel
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        [Attribute.Field(IsPrimaryKey = true)]
        public int Id
        {
            get;
            set;
        }
        [System.Xml.Serialization.XmlIgnore]
        [NonSerialized]
        private DateTime addTime = DateTime.Now;

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime
        {
            get
            {
                return addTime;
            }
            set
            {
                addTime = value;
            }
        }

    }
    /// <summary>
    /// 基类,不包含任何字段
    /// 如果有自定义主键名对象,请继承此类型
    /// </summary>
    [Serializable]
    //[Attribute.ModelProxy]
    public abstract class IModel : ICloneable
    {
        protected static object lockObj = new object();
        public string ToJson()
        {
            return CoreHelper.StringHelper.SerializerToJson(this);
        }
        /// <summary>
        /// 数据校验方法
        /// </summary>
        /// <returns></returns>
        public virtual string CheckData()
        {
            return "";
        }
        /// <summary>
        /// 创建表时的初始数据
        /// </summary>
        /// <returns></returns>
        public virtual System.Collections.IList GetInitData()
        {
            return null;
        }
        [System.Xml.Serialization.XmlIgnore]
        [NonSerialized]
        Dictionary<string, dynamic> Datas = new Dictionary<string, dynamic>();

        /// <summary>
        /// 是否检查重复插入,默认为true
        /// 判断重复为相同的属性值,AddTime除外,3秒内唯一
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        protected internal virtual bool CheckRepeatedInsert
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 获取关联查询的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Attribute.Field(MapingField = false)]
        public dynamic this[string key]
        {
            get
            {
                dynamic obj = null;
                var a = Datas.TryGetValue(key.ToLower(), out obj);
                if (!a)
                {
                    throw new Exception(string.Format("对象:{0}不存在索引值:{1}", GetType(), key));
                }
                return obj;
            }
            set
            {
                Datas[key.ToLower()] = value;
            }
        }

        #region 检查表
        /// <summary>
        /// 检查索引
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public void CheckIndexExists(DBExtend helper)
        {
            var list = GetIndexScript(helper);
            foreach (var item in list)
            {
                try
                {
                    helper.Execute(item);
                }
                catch (Exception ero)//出错,
                {
                    CoreHelper.EventLog.Log(string.Format("创建索引失败:{0}\r\n{1}", ero.Message, item));
                }
            }
        }
        internal static string CreateColumn(DBExtend helper, Attribute.FieldAttribute item)
        {
            var dbAdapter = helper._DBAdapter;
            string result = "";
            if (string.IsNullOrEmpty(item.ColumnType))
            {
                throw new Exception("ColumnType is null");
            }
            string str = dbAdapter.GetCreateColumnScript(item);
            string indexScript = "";
            if (item.FieldIndexType != Attribute.FieldIndexType.无)
            {
                indexScript = dbAdapter.GetColumnIndexScript(item);
            }
            try
            {
                helper.Execute(str);
                if (!string.IsNullOrEmpty(indexScript))
                {
                    helper.Execute(indexScript);
                }
                result = string.Format("创建字段:{0} {1} {2}\r\n", item.TableName, item.Name,item.PropertyType);
                CoreHelper.EventLog.Log(result, "", false);
            }
            catch (Exception ero)
            {
                //CoreHelper.EventLog.Log("创建字段时发生错误:" + ero.Message);
                result = string.Format("创建字段:{0} {1}发生错误:{2}\r\n", item.TableName, item.Name, ero.Message);
            }
            return result;
        }
        /// <summary>
        /// 检查对应的字段是否存在,不存在则创建
        /// </summary>
        /// <param name="helper"></param>
        public string CheckColumnExists(DBExtend helper)
        {
            string result = "";
            var dbAdapter = helper._DBAdapter;
            List<Attribute.FieldAttribute> columns = GetColumns(dbAdapter);
            string tableName = TypeCache.GetTableName(this.GetType(),helper.dbContext);
            foreach (Attribute.FieldAttribute item in columns)
            {
                string sql = dbAdapter.GetSelectTop(item.KeyWordName, "from " + tableName, "", 1);
                try
                {
                    helper.Execute(sql);
                }
                catch//出错,按没有字段算
                {
                    result += CreateColumn(helper, item);
                    
                }
            }
            return result;
        }
        internal static void SetColumnDbType(DBAdapter.DBAdapterBase dbAdapter, Attribute.FieldAttribute info)
        {
            if (info.FieldType != Attribute.FieldType.数据库字段)
            {
                return ;
            }
            string defaultValue;
            Type propertyType = info.PropertyType;
            var columnType = dbAdapter.GetColumnType(info, out defaultValue);
            info.ColumnType = columnType;
            info.DefaultValue = defaultValue;
            if (info.ColumnType.Contains("{0}"))
            {
                throw new Exception(string.Format("属性:{0} 需要指定长度 ColumnType:{1}", info.Name, info.ColumnType));
            }
        }
        /// <summary>
        /// 获取列
        /// </summary>
        /// <returns></returns>
        List<Attribute.FieldAttribute> GetColumns(DBAdapter.DBAdapterBase dbAdapter)
        {
            //var dbAdapter = Base.CurrentDBAdapter;
            Type type = this.GetType();
            string tableName = TypeCache.GetTableName(type,dbAdapter.dbContext);
            var typeArry = TypeCache.GetProperties(type, true).Values;
            var columns = new List<CRL.Attribute.FieldAttribute>();
            foreach (var info in typeArry)
            {
                if (info.FieldType == Attribute.FieldType.虚拟字段)
                    continue;
                SetColumnDbType(dbAdapter, info);
                columns.Add(info);
            }
            return columns;
        }
        internal List<string> GetIndexScript(DBExtend helper)
        {
            var dbAdapter = helper._DBAdapter;
            List<string> list2 = new List<string>();
            List<Attribute.FieldAttribute> columns = GetColumns(dbAdapter);
            foreach (Attribute.FieldAttribute item in columns)
            {
                if (item.FieldIndexType != Attribute.FieldIndexType.无)
                {
                    //string indexScript = string.Format("CREATE {2} NONCLUSTERED INDEX  IX_INDEX_{0}_{1}  ON dbo.[{0}]({1})", tableName, item.Name, item.FieldIndexType == Attribute.FieldIndexType.非聚集唯一 ? "UNIQUE" : "");
                    string indexScript = dbAdapter.GetColumnIndexScript(item);
                    list2.Add(indexScript);
                }
            }
            return list2;
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public string CreateTable(DBExtend helper)
        {
            string msg;
            CreateTable(helper, out msg);
            return msg;
        }
        /// <summary>
        /// 创建表
        /// 会检查表是否存在,如果存在则检查字段
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool CreateTable(DBExtend helper, out string message)
        {
            var dbAdapter = helper._DBAdapter;
            message = "";
            //TypeCache.SetDBAdapterCache(GetType(),dbAdapter);
            string tableName = TypeCache.GetTableName(GetType(),helper.dbContext);
            string sql = dbAdapter.GetSelectTop("0", "from " + tableName, "", 1);
            bool needCreate = false;
            try
            {
                //检查表是否存在
                helper.Execute(sql);
            }
            catch
            {
                needCreate = true;
            }
            if (needCreate)
            {
                List<string> list = new List<string>();
                try
                {
                    List<Attribute.FieldAttribute> columns = GetColumns(dbAdapter);
                    dbAdapter.CreateTable(columns, tableName);
                    message = string.Format("创建表:{0}\r\n", tableName);
                    CheckIndexExists(helper);
                    return true;
                }
                catch (Exception ero)
                {
                    message = "创建表时发生错误 类型{0} {1}\r\n";
                    message = string.Format(message, GetType(), ero.Message);
                    throw new Exception(message);
                    return false;
                }
                CoreHelper.EventLog.Log(message, "", false);
            }
            else
            {
                message = CheckColumnExists(helper);
            }
            return true;
        }

        #endregion

        #region 更新值判断
        /// <summary>
        /// 存放原始克隆
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [NonSerialized]
        private object _originClone = null;

        [System.Xml.Serialization.XmlIgnore]
        [Attribute.Field(MapingField = false)]
        internal object OriginClone
        {
            get { return _originClone; }
            set { _originClone = value; }
        }

        [System.Xml.Serialization.XmlIgnore]
        [NonSerialized]
        bool boundChange = true;

        [System.Xml.Serialization.XmlIgnore]
        internal bool BoundChange
        {
            get
            {
                return boundChange;
            }
            set
            {
                boundChange = value;
            }
        }
        [System.Xml.Serialization.XmlIgnore]
        [NonSerialized]
        ParameCollection changes = new ParameCollection();

        [Attribute.Field(MapingField = false)]
        [System.Xml.Serialization.XmlIgnore]
        internal ParameCollection Changes
        {
            get
            {
                return changes;
            }
            set
            {
                changes = value;
            }
        }
        /// <summary>
        /// 表示值被更改了
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        internal protected void SetChanges(string name,object value)
        {
            if (!BoundChange)
                return;
            if (name.ToLower() == "boundchange")
                return;
            Changes[name] = value;
        }
        #endregion
        /// <summary>
        /// 创建当前对象的浅表副本
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        [System.Xml.Serialization.XmlIgnore]
        [NonSerialized]
        string modelKey = null;

        internal string GetModelKey()
        {
            if (modelKey == null)
            {
                var type = GetType();
                var tab = TypeCache.GetTable(type);
                modelKey = string.Format("{0}_{1}", type, tab.PrimaryKey.GetValue(this));
            }
            return modelKey;
        }
        internal int GetpPrimaryKeyValue()
        {
            var primaryKey = TypeCache.GetTable(GetType()).PrimaryKey;
            var keyValue = (int)primaryKey.GetValue(this);
            return keyValue;
        }
    }
}
