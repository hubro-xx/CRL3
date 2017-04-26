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
using System.Reflection;
using System.Dynamic;
using Newtonsoft.Json;

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
    public abstract class IModel 
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
        /// ToJson
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[" + GetType().FullName + "] " + ToJson();
        }
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
        /// 手动跟踪对象状态,使更新时能识别
        /// </summary>
        public void BeginTracking()
        {
            OriginClone = Clone();
            Changes = new ParameCollection();
        }

        /// <summary>
        /// 是否检查重复插入,默认为true
        /// 判断重复为相同的属性值,AddTime除外,3秒内唯一
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [JsonIgnore]
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
        Dictionary<string, object> Datas = null;


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
                Datas = Datas ?? new Dictionary<string, object>();
                var a = Datas.TryGetValue(key.ToLower(), out obj);
                if (!a)
                {
                    throw new CRLException(string.Format("对象:{0}不存在索引值:{1}", GetType(), key));
                }
                return obj;
            }
            set
            {
                Datas = Datas ?? new Dictionary<string, object>();
                Datas[key.ToLower()] = value;
            }
        }
        /// <summary>
        /// 设置索引值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetIndexData(string key, object value)
        {
            Datas = Datas ?? new Dictionary<string, object>();
            Datas[key.ToLower()] = value;
        }
        #endregion


        #region 更新值判断
        /// <summary>
        /// 存放原始克隆
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [NonSerialized]
        internal object OriginClone = null;

        [System.Xml.Serialization.XmlIgnore]
        [NonSerialized]
        internal bool BoundChange = true;

        /// <summary>
        /// 存储被更改的属性
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        ParameCollection Changes = null;
        /// <summary>
        /// 表示值被更改了
        /// 当更新后,将被清空
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        internal protected void SetChanges(string name,object value)
        {
            if (!BoundChange)
                return;
            if (name.ToLower() == "boundchange")
                return;
            Changes = Changes ?? new ParameCollection();
            Changes[name] = value;
        }
        ParameCollection GetChanges()
        {
            Changes = Changes ?? new ParameCollection();
            return Changes;
        }
        /// <summary>
        /// 清空Changes并重新Clone源对象
        /// </summary>
        internal void CleanChanges()
        {
            Changes.Clear();
            if (SettingConfig.UsePropertyChange)
            {
                return;
            }
            OriginClone = Clone();
        }
        /// <summary>
        /// 获取被修改的字段
        /// </summary>
        /// <returns></returns>
        public ParameCollection GetUpdateField(bool check = true)
        {
            var c = new ParameCollection();
            var fields = TypeCache.GetProperties(GetType(), true);
            if (this.GetChanges().Count > 0)//按手动指定更改
            {
                foreach (var item in this.GetChanges())
                {
                    var key = item.Key.Replace("$", "");
                    var f = fields[key];
                    if (f == null)
                        continue;
                    if (f.IsPrimaryKey || f.FieldType != Attribute.FieldType.数据库字段)
                        continue;
                    var value = item.Value;
                    //如果表示值为被追加 名称为$name
                    //使用Cumulation扩展方法后按此处理
                    if (key != item.Key)//按$name=name+'123123'
                    {
                        if (f.PropertyType == typeof(string))
                        {
                            value = string.Format("{0}+'{1}'", key, value);
                        }
                        else
                        {
                            value = string.Format("{0}+{1}", key, value);
                        }
                    }
                    c[item.Key] = value;
                }
                return c;
            }
            //按对象对比
            var origin = this.OriginClone;
            if (origin == null && check)
            {
                throw new CRLException("_originClone为空,请确认此对象是由查询创建");
            }
            foreach (var f in fields.Values)
            {
                if (f.IsPrimaryKey)
                    continue;
                var originValue = f.GetValue(origin);
                var currentValue = f.GetValue(this);
                if (!Object.Equals(originValue, currentValue))
                {
                    c.Add(f.MemberName, currentValue);
                }
            }
            return c;
        }
        /// <summary>
        /// 对象是否被修改
        /// </summary>
        public bool IsModified()
        {
            return GetUpdateField(false).Count > 0;
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

        internal string GetModelKey()
        {
            var type = GetType();
            var tab = TypeCache.GetTable(type);
            var modelKey = string.Format("{0}_{1}", type, tab.PrimaryKey.GetValue(this));
            return modelKey;
        }
        internal object GetpPrimaryKeyValue()
        {
            var primaryKey = TypeCache.GetTable(GetType()).PrimaryKey;
            var keyValue = primaryKey.GetValue(this);
            return keyValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public string CreateTable(AbsDBExtend db)
        {
            return ModelCheck.CreateTable(GetType(), db);
        }
        #region 动态字典,效果同索引
        private Dynamic.DynamicViewDataDictionary _dynamicViewDataDictionary;

        /// <summary>
        /// 动态Bag,可用此取索引值
        /// 不区分大小写
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [JsonIgnore]
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
