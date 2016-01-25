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
        /// </summary>
        internal static ConcurrentDictionary<Type, string> ModelKeyCache = new ConcurrentDictionary<Type, string>();


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
                    throw new Exception("未设置分表定位索引,dbContext.ShardingMainDataIndex");
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
            if (typeCache.ContainsKey(type))
                return typeCache[type];
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
        //internal static void SetDBAdapterCache(Type type, DBAdapter.DBAdapterBase dBAdapter)
        //{
        //    var table = GetTable(type);
        //    if (table.DBAdapter != null)
        //    {
        //        return;
        //    }
        //    table.DBAdapter = dBAdapter;
        //}
        //internal static DBAdapter.DBAdapterBase GetDBAdapterFromCache(Type type)
        //{
        //    var table = GetTable(type);
        //    if (table.DBAdapter == null)
        //        throw new Exception("未初始对应的适配器,在类型:" + type);
        //    return table.DBAdapter;
        //}
        /// <summary>
        /// 获取字段,并指定是否为基本查询字段(包函虚拟字段)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onlyField">是否为基本查询字段</param>
        /// <returns></returns>
        public static IgnoreCaseDictionary<Attribute.FieldAttribute> GetProperties(Type type, bool onlyField)
        {
            var table = GetTable(type);
            var list = new IgnoreCaseDictionary<Attribute.FieldAttribute>();
            foreach (var item in table.Fields)
            {
                if (onlyField)
                {
                    if (item.FieldType != Attribute.FieldType.数据库字段 && item.FieldType != Attribute.FieldType.虚拟字段)
                    {
                        continue;
                    }
                }
                list.Add(item.Name, item.Clone());
            }
            return list;
        }
        static void SetProperties(Attribute.TableAttribute table)
        {
            if (table.Fields.Count > 0)
            {
                return;
            }
            var type = table.Type;
            List<Attribute.FieldAttribute> list = new List<CRL.Attribute.FieldAttribute>();
            //string fieldPat = @"^([A-Z][a-z|\d]+)+$";
            int n = 0;
            #region 读取
            List<PropertyInfo> typeArry = table.Type.GetProperties().ToList();
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
                //    throw new Exception(string.Format("属性名:{0} 不符合规则:{1}", info.Name, fieldPat));
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
                f.Name = info.Name;
                f.TableName = table.TableName;
                f.ModelType = table.Type;
                if (!string.IsNullOrEmpty(f.VirtualField))
                {
                    if (SettingConfig.StringFormat != null)
                    {
                        f.VirtualField = SettingConfig.StringFormat(f.VirtualField);
                    }
                    f.VirtualField = f.VirtualField.Replace("$", "{" + type.FullName + "}");
                }
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
                    n += 1;
                }
                list.Add(f);
            }
            if (n == 0)
            {
                //throw new Exception(string.Format("对象{0}未设置任何主键", type.Name));
            }
            else if (n > 1)
            {
                throw new Exception(string.Format("对象{0}设置的主键字段太多 {1}", type.Name, n));
            }
            #endregion
            table.Fields = list;
        }
    }
}
