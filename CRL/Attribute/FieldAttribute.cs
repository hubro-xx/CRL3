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

namespace CRL.Attribute
{
    /// <summary>
    /// 字段属性设置
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : System.Attribute
    {
        public FieldAttribute Clone()
        {
            return MemberwiseClone() as FieldAttribute;
        }
        bool isPrimaryKey = false;
        /// <summary>
        /// 当是主键时,是否保留原标识,和IsPrimaryKey一起用
        /// 在插入和生成表脚本时,会按此判断
        /// </summary>
        public bool KeepIdentity { get; set; }
        /// <summary>
        /// 是否为主键
        /// 名称为id自动为主键
        /// </summary>
        public bool IsPrimaryKey
        {
            get
            {
                if (MapingName.ToLower() == "id")
                {
                    return true;
                }
                return isPrimaryKey;
            }
            set { isPrimaryKey = value; }
        }
        FieldType fieldType = FieldType.NONE;
        /// <summary>
        /// 字段类型
        /// </summary>
        public FieldType FieldType
        {
            get
            {
                if (fieldType == Attribute.FieldType.NONE)
                {
                    //var isSystemType = PropertyType.Namespace == "System" || PropertyType.BaseType.Name == "Enum";
                    //if (!string.IsNullOrEmpty(VirtualField))
                    //{
                    //    fieldType= Attribute.FieldType.虚拟字段;
                    //}
                    //else
                        if (!string.IsNullOrEmpty(ConstraintField))
                    {
                        //fieldType = isSystemType ? FieldType.关联字段 : Attribute.FieldType.关联对象;
                        fieldType = Attribute.FieldType.关联字段;
                    }
                    else
                    {
                        fieldType = Attribute.FieldType.数据库字段;
                    }
                }
                return fieldType;
            }
        }
        
        /// <summary>
        /// 索引类型
        /// </summary>
        public FieldIndexType FieldIndexType { get; set; }
        bool mapingField = true;
        /// <summary>
        /// 是否映射该字段
        /// 为false时则不参与查询
        /// </summary>
        public bool MapingField
        {
            get
            {
                return mapingField;
            }
            set
            {
                mapingField = value;
            }
        }
        public override string ToString()
        {
            return string.Format("{0}.{1} |{2}", TableName, MemberName, QueryFullScript);
        }
        /// <summary>
        /// 属性名称
        /// </summary>
        internal string MemberName;
        public string GetMemberName()
        {
            return MemberName;
        }

        /// <summary>
        /// 对象类型
        /// </summary>
        internal Type ModelType;

        string mapingName;
        /// <summary>
        /// 映射字段名
        /// MongoDB不支持表和字段别名
        /// </summary>
        public string MapingName
        {
            get
            {
                if (string.IsNullOrEmpty(mapingName))
                    mapingName = MemberName;
                return mapingName;
            }
            set { 
                mapingName = value;
            }
        }

      
        /// <summary>
        /// 查询完整语句
        /// </summary>
        internal string QueryFullScript;
        /// <summary>
        /// 查询的完整字段名,t1.Name
        /// </summary>
        internal string QueryField;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_DBAdapter"></param>
        /// <param name="usePrefix">是否使用字段前辍 如 t1.Name</param>
        /// <param name="withTablePrefix">是否生按表生成前辍,关联时用 如Table__Name</param>
        /// <param name="mapingName">别名,空则按字段名,没有AS</param>
        /// <param name="fieldName">自定义查询字段名,空则按Name</param>
        internal void SetFieldQueryScript2(DBAdapter.DBAdapterBase _DBAdapter, string usePrefix, bool withTablePrefix, string mapingName,string fieldName="")
        {
            string query = "";
            if (!string.IsNullOrEmpty(usePrefix))
            {
                //query += "{" + ModelType.FullName + "}";
                query += usePrefix;
            }
            this.FieldMapping = new FieldMapping() { QueryName = MapingName, MappingName = mapingName, PropertyType = PropertyType, WithTablePrefix = withTablePrefix };
            if (!string.IsNullOrEmpty(mapingName))
            {
                FieldMapping.QueryName = mapingName;
                FieldMapping.MappingName = mapingName;
            }
            else
            {
                FieldMapping.QueryName = MapingName;
                FieldMapping.MappingName = MemberName;
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                fieldName = withTablePrefix ? MapingName : _DBAdapter.KeyWordFormat(MapingName);
            }
            //判断虚拟字段
            //if (FieldType == Attribute.FieldType.虚拟字段)
            //{
            //    query = VirtualField.Replace("{" + ModelType.FullName + "}", usePrefix);//替换前辍
            //    mapingName = MemberName;
            //}
            //else
            //{
            //    query += fieldName;
            //}
            query += fieldName;
            QueryField = query;
            var mappNameFull = fieldName;
         
            if (!string.IsNullOrEmpty(mapingName))
            {
                //MapingName = mapingName;
                mappNameFull = mapingName;
            }
 
            if (withTablePrefix)
            {
                FieldMapping.MappingName = mappNameFull;
                mappNameFull = GetTableFieldFormat(TableName, mappNameFull);
                FieldMapping.QueryName = mappNameFull;
            }

            //MapingName = mappNameFull;
            //别名不为空或有表前辍
            if (!string.IsNullOrEmpty(mapingName) || withTablePrefix)
            {
                QueryFullScript = string.Format("{0} as {1}", query, mappNameFull);
            }
            else
            {
                QueryFullScript = query;
            }
        }
        /// <summary>
        /// 按表名格式化字段名
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fileld"></param>
        /// <returns></returns>
        internal string GetTableFieldFormat(string table, string fileld)
        {
            return string.Format("{0}__{1}", table, fileld);
        }
        //internal string ModelRemark;
        /// <summary>
        /// 备注
        /// </summary>
        internal string Remark;
        /// <summary>
        /// 主表名
        /// </summary>
        internal string TableName;

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 自定义数据库字段类型,如 varchar(50)
        /// </summary>
        public string ColumnType { get; set; }
        bool notNull;
        /// <summary>
        /// 不可为空,主键自动为不可为空
        /// </summary>
        public bool NotNull
        {
            get
            {
                if (IsPrimaryKey)
                {
                    return true;
                }
                return notNull;
            }
            set
            {
                notNull = value;
            }
        }
        int length = 30;
        /// <summary>
        /// 长度,超过3000字段类型将会设为ntext
        /// 若是需要指定长度,请赋值
        /// 默认30
        /// </summary>
        public int Length { get { return length; } set { length = value; } }
        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType;
        #region 约束 自动关查询时用
        /// <summary>
        /// 自动转换虚拟字段
        /// 如year($addtime)
        /// 字段前需加前辍$,以在关联查询时区分
        /// </summary>
        ///public string VirtualField { get; set; }
        /// <summary>
        /// 约束字段
        /// 格式:$CategoryCode[当前类型字段]=SequenceCode[关联表字段]
        /// </summary>
        public string ConstraintField { get; set; }
        /// <summary>
        /// 子表查询附加条件
        /// 如:CategoryCode=1
        /// </summary>
        public string Constraint { get; set; }
        /// <summary>
        /// 关联表类型
        /// 只是字段时使用
        /// typeof(ClassA)
        /// </summary>
        public Type ConstraintType { get; set; }
        /// <summary>
        /// 关联表要取出的字段
        /// 只是字段时使用
        /// </summary>
        public string ConstraintResultField { get; set; }
        #endregion
        PropertyInfo propertyInfo;
        /// <summary>
        /// 设置对象属性值
        /// </summary>
        /// <param name="_propertyInfo"></param>
        internal void SetPropertyInfo(PropertyInfo _propertyInfo)
        {
            propertyInfo = _propertyInfo;
        }
        internal PropertyInfo GetPropertyInfo()
        {
            return propertyInfo;
        }
        /// <summary>
        /// 获取对象属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal object GetValue(object obj)
        {
            return propertyInfo.GetValue(obj, null);
        }
        /// <summary>
        /// 用反射设置对象属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        internal void SetValue(object obj, object value)
        {
            if (value == null)
                return;
            if (value is DBNull)
                return;
            //Type type = value.GetType();
            //if (propertyInfo.PropertyType != type)
            //{
            //    if (value is Int32 && propertyInfo.PropertyType==typeof(string))
            //    {
            //        value = value.ToString();
            //    }
            //}
            //try
            //{
            //    //oracle会出现类型转换问题
            //    value = ObjectConvert.ConvertObject(propertyInfo.PropertyType, value);
            //}
            //catch(Exception ero)
            //{
            //    throw new CRLException(ero.Message + " 在属性" + propertyInfo.Name + " " + propertyInfo.PropertyType);
            //}
            propertyInfo.SetValue(obj, value, null);
        }
        /// <summary>
        /// MongoDB用
        /// </summary>
        internal FieldQuery FieldQuery;
        internal FieldMapping FieldMapping;
    }
    struct FieldMapping
    {
        /// <summary>
        /// 查询名
        /// </summary>
        public string QueryName;
        /// <summary>
        /// 映射名
        /// </summary>
        public string MappingName;
        public override string ToString()
        {
            return string.Format("Q:{0} M:{1}", QueryName, MappingName);
        }
        public Type PropertyType;
        public bool WithTablePrefix;
    }
    struct FieldQuery
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName;
        /// <summary>
        /// 成员名,别名
        /// </summary>
        public string MemberName;
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName;
    }

}
