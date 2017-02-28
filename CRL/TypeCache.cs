/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CRL
{
    internal class TypeCache
    {
        //static object lockObj = new object();
        internal static ConcurrentDictionary<Type, Attribute.TableAttribute> typeCache = new ConcurrentDictionary<Type, Attribute.TableAttribute>();
        /// <summary>
        /// 对象类型缓存
        /// 类型+key
        /// </summary>
        static ConcurrentDictionary<string, string> ModelKeyCache = new ConcurrentDictionary<string, string>();

        #region modelKey
        public static bool GetModelKeyCache(Type type, string dataBase, out string key)
        {
            var typeKey = string.Format("{0}|{1}", type, dataBase);
            return ModelKeyCache.TryGetValue(typeKey, out key);
        }
        public static void SetModelKeyCache(Type type, string dataBase,  string key)
        {
            var typeKey = string.Format("{0}|{1}", type, dataBase);
            ModelKeyCache[typeKey] = key;
        }
        public static void RemoveModelKeyCache(Type type, string dataBase)
        {
            var typeKey = string.Format("{0}|{1}", type, dataBase);
            string val;
            ModelKeyCache.TryRemove(typeKey, out val);
        }
        #endregion
        /// <summary>
        /// 根据类型返回表名
        /// 如果设置了分表,返回分表名
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static string GetTableName(Type type, DbContext dbContext)
        {
            var tableName = GetTable(type).TableName;
            return GetTableName(tableName, dbContext);
        }
        public static string GetTableName(string tableName, DbContext dbContext)
        {
            if (dbContext != null && dbContext.UseSharding)
            {
                if (dbContext.ShardingMainDataIndex == 0)
                {
                    throw new CRLException("未设置分表定位索引,dbContext.ShardingMainDataIndex");
                }
                var location = Sharding.DBService.GetLocation(tableName, dbContext.ShardingMainDataIndex, dbContext.DBLocation.ShardingDataBase);
                tableName = location.TablePart.PartName;
            }
            return tableName;
        }
        /// <summary>
        /// 获取表属性,如果要获取表名,调用GetTableName方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Attribute.TableAttribute GetTable(Type type)
        {
            Attribute.TableAttribute table;
            var b = typeCache.TryGetValue(type, out table);
            if (b)
            {
                if (table.Fields.Count > 0)
                {
                    return table;
                }
            }
            object[] objAttrs = type.GetCustomAttributes(typeof(Attribute.TableAttribute), true);
            Attribute.TableAttribute des;
            if (objAttrs == null || objAttrs.Length == 0)
            {
                des = new Attribute.TableAttribute() { TableName = type.Name };
            }
            else
            {
                des = objAttrs[0] as Attribute.TableAttribute;
            }
            des.Type = type;
            //lock (lockObj)
            //{
            //    if (!typeCache.ContainsKey(type))
            //    {
            //        typeCache.Add(type, des);
            //    }
            //}
            if (!typeCache.ContainsKey(type))
            {
                typeCache.TryAdd(type, des);
            }
            if (string.IsNullOrEmpty(des.TableName))
            {
                des.TableName = type.Name;
            }
            SetProperties(des);
            return des;
        }
        /// <summary>
        /// 获取数据库字段,包函虚拟字段
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onlyField"></param>
        /// <returns></returns>
        public static IgnoreCaseDictionary<Attribute.FieldAttribute> GetProperties(Type type, bool onlyField)
        {
            var table = GetTable(type);
            return table.FieldsDic;
            //var list = new IgnoreCaseDictionary<Attribute.FieldAttribute>();
            //foreach (var item in table.Fields)
            //{
            //    if (onlyField)
            //    {
            //        if (item.FieldType == Attribute.FieldType.关联字段)
            //        {
            //            continue;
            //        }
            //    }
            //    list.Add(item.MemberName, item.Clone());
            //}
            //return list;
        }
        static void SetProperties(Attribute.TableAttribute table)
        {
            if (table.Fields.Count > 0)
            {
                return;
            }
            var type = table.Type;
            List<Attribute.FieldAttribute> list = new List<CRL.Attribute.FieldAttribute>();
            var fieldDic = new IgnoreCaseDictionary<Attribute.FieldAttribute>();
            //string fieldPat = @"^([A-Z][a-z|\d]+)+$";
            int n = 0;
            Attribute.FieldAttribute keyField = null;
            #region 读取
            var typeArry = table.Type.GetProperties().ToList();
            //移除重复的
            var dic = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo info in typeArry)
            {
                if (!dic.ContainsKey(info.Name))
                {
                    dic.Add(info.Name, info);
                }
            }
            foreach (PropertyInfo info in dic.Values)
            {
                //if (!System.Text.RegularExpressions.Regex.IsMatch(info.Name, fieldPat))
                //{
                //    throw new CRLException(string.Format("属性名:{0} 不符合规则:{1}", info.Name, fieldPat));
                //}
                //排除没有SET方法的属性
                if (info.GetSetMethod() == null)
                {
                    continue;
                }
                Type propertyType = info.PropertyType;
                Attribute.FieldAttribute f = new CRL.Attribute.FieldAttribute();
                //排除集合类型
                if (propertyType.FullName.IndexOf("System.Collections") > -1)
                {
                    continue;
                }

                object[] attrs = info.GetCustomAttributes(typeof(Attribute.FieldAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    f = attrs[0] as Attribute.FieldAttribute;
                }
                f.SetPropertyInfo(info);
                f.PropertyType = propertyType;
                f.MemberName = info.Name;
                f.TableName = table.TableName;
                f.ModelType = table.Type;
                //if (!string.IsNullOrEmpty(f.VirtualField))
                //{
                //    if (SettingConfig.StringFormat != null)
                //    {
                //        f.VirtualField = SettingConfig.StringFormat(f.VirtualField);
                //    }
                //    f.VirtualField = f.VirtualField.Replace("$", "{" + type.FullName + "}");//虚拟字段使用Type名
                //}
                //排除不映射字段
                if (!f.MapingField)
                {
                    continue;
                }
                if (propertyType == typeof(System.String))
                {
                    if (f.Length == 0)
                        f.Length = 30;
                }
                if (f.IsPrimaryKey)//保存主键
                {
                    table.PrimaryKey = f;
                    f.FieldIndexType = Attribute.FieldIndexType.非聚集唯一;
                    keyField = f;
                    n += 1;
                }
                if (f.FieldType != Attribute.FieldType.关联字段)
                {
                    fieldDic.Add(f.MemberName, f);
                }
                list.Add(f);
            }
            if (n == 0)
            {
                //throw new CRLException(string.Format("对象{0}未设置任何主键", type.Name));
            }
            else if (n > 1)
            {
                throw new CRLException(string.Format("对象{0}设置的主键字段太多 {1}", type.Name, n));
            }
            #endregion
            //主键排前面
            if (keyField != null)
            {
                list.Remove(keyField);
                list.Insert(0, keyField);
            }
            table.Fields = list;
            table.FieldsDic = fieldDic;
        }
    }
}
