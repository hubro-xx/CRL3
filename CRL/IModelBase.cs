using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Dynamic;

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
        /// <summary>
        /// 序列化为JSON
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return CoreHelper.StringHelper.SerializerToJson(this);
        }

        #region 方法重写
        /// <summary>
        /// 数据校验方法,可重写
        /// </summary>
        /// <returns></returns>
        public virtual string CheckData()
        {
            return "";
        }

        /// <summary>
        /// 当列创建时,可重写
        /// 可处理在添加字段后数据的升级
        /// </summary>
        /// <param name="fieldName"></param>
        protected internal virtual void OnColumnCreated(string fieldName)
        {
        }
        /// <summary>
        /// 创建表时的初始数据,可重写
        /// </summary>
        /// <returns></returns>
        protected internal virtual System.Collections.IList GetInitData()
        {
            return null;
        }
        #endregion


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
        #region 索引

        [System.Xml.Serialization.XmlIgnore]
        [NonSerialized]
        Dictionary<string, dynamic> Datas = new Dictionary<string, dynamic>();


        /// <summary>
        /// 获取关联查询的值
        /// 不分区大小写
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
        #endregion

        #region 检查表
        /// <summary>
        /// 检查索引
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        internal void CheckIndexExists(DBExtend db)
        {
            var list = GetIndexScript(db);
            foreach (var item in list)
            {
                try
                {
                    db.Execute(item);
                }
                catch (Exception ero)//出错,
                {
                    CoreHelper.EventLog.Log(string.Format("创建索引失败:{0}\r\n{1}", ero.Message, item));
                }
            }
        }
        /// <summary>
        /// 创建列
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static string CreateColumn(DBExtend db, Attribute.FieldAttribute item)
        {
            var dbAdapter = db._DBAdapter;
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
                db.Execute(str);
                if (!string.IsNullOrEmpty(indexScript))
                {
                    db.Execute(indexScript);
                }
                result = string.Format("创建字段:{0} {1} {2}\r\n", item.TableName, item.Name,item.PropertyType);
                var model = System.Activator.CreateInstance(item.ModelType) as IModel;
                try
                {
                    model.OnColumnCreated(item.Name);
                }
                catch(Exception ero)
                {
                    result = string.Format("添加字段:{0} {1},升级数据时发生错误:{2}\r\n", item.TableName, item.Name, ero.Message);
                }
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
        /// <param name="db"></param>
        internal string CheckColumnExists(DBExtend db)
        {
            string result = "";
            var dbAdapter = db._DBAdapter;
            List<Attribute.FieldAttribute> columns = GetColumns(dbAdapter);
            string tableName = TypeCache.GetTableName(this.GetType(),db.dbContext);
            foreach (Attribute.FieldAttribute item in columns)
            {
                string sql = dbAdapter.GetSelectTop(item.KeyWordName, "from " + tableName, "", 1);
                try
                {
                    db.Execute(sql);
                }
                catch//出错,按没有字段算
                {
                    result += CreateColumn(db, item);
                    
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
                if (info.FieldType != Attribute.FieldType.数据库字段)
                    continue;
                SetColumnDbType(dbAdapter, info);
                columns.Add(info);
            }
            return columns;
        }
        internal List<string> GetIndexScript(DBExtend db)
        {
            var dbAdapter = db._DBAdapter;
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
        /// <param name="db"></param>
        /// <returns></returns>
        public string CreateTable(DBExtend db)
        {
            string msg;
            CreateTable(db, out msg);
            return msg;
        }
        /// <summary>
        /// 创建表
        /// 会检查表是否存在,如果存在则检查字段
        /// 创建失败则抛出异常
        /// </summary>
        /// <param name="db"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        internal void CreateTable(DBExtend db, out string message)
        {
            var dbAdapter = db._DBAdapter;
            message = "";
            //TypeCache.SetDBAdapterCache(GetType(),dbAdapter);
            string tableName = TypeCache.GetTableName(GetType(),db.dbContext);
            string sql = dbAdapter.GetSelectTop("0", "from " + tableName, "", 1);
            bool needCreate = false;
            try
            {
                //检查表是否存在
                db.Execute(sql);
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
                    CheckIndexExists(db);
                    //return true;
                }
                catch (Exception ero)
                {
                    message = "创建表时发生错误 类型{0} {1}\r\n";
                    message = string.Format(message, GetType(), ero.Message);
                    throw new Exception(message);
                    //return false;
                }
                //CoreHelper.EventLog.Log(message, "", false);
            }
            else
            {
                message = CheckColumnExists(db);
            }
            //return false;
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
        internal string GetpPrimaryKeyValue()
        {
            var primaryKey = TypeCache.GetTable(GetType()).PrimaryKey;
            var keyValue = primaryKey.GetValue(this);
            return keyValue.ToString();
        }

        #region 动态字典,效果同索引
        private Dynamic.DynamicViewDataDictionary _dynamicViewDataDictionary;

        /// <summary>
        /// 动态Bag,可用此取索引值
        /// 不区分大小写
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public dynamic Bag
        {
            get
            {
                if (this._dynamicViewDataDictionary == null)
                {
                    this._dynamicViewDataDictionary = new Dynamic.DynamicViewDataDictionary(Datas);
                }
                return this._dynamicViewDataDictionary;
            }
        }

        #endregion

    }
}
