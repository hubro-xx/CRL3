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
using System.Linq.Expressions;
using System.Text;
using CoreHelper;
using CRL.LambdaQuery;
namespace CRL.DBExtend.RelationDB
{
    public sealed partial class DBExtend
    {
        /// <summary>
        /// 格式化为更新值查询
        /// </summary>
        /// <param name="setValue"></param>
        /// <param name="joinType"></param>
        /// <returns></returns>
        string ForamtSetValue<T>(ParameCollection setValue, Type joinType = null) where T : IModel
        {
            //string tableName = TypeCache.GetTableName(typeof(T), dbContext);
            string setString = "";
            var fields = TypeCache.GetProperties(typeof(T), true);
            foreach (var pair in setValue)
            {
                string name = pair.Key;
                object value = pair.Value;

                value = ObjectConvert.CheckNullValue(value);

                if (name.StartsWith("$"))//直接按值拼接 c2["$SoldCount"] = "SoldCount+" + num;
                {
                    name = name.Substring(1, name.Length - 1);
                    if (!fields.ContainsKey(name))
                    {
                        throw new CRLException("找不到对应的字段,在" + typeof(T) + ",名称" + name);
                    }
                    var field = fields[name];
                    string value1 = value.ToString();
                    //未处理空格
                    value1 = System.Text.RegularExpressions.Regex.Replace(value1, name + @"([\+\-])", field.MapingName + "$1", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    name = field.MapingName;
                    value = value1;
                    setString += string.Format(" {0}={1},", _DBAdapter.KeyWordFormat(name), value);
                }
                else
                {
                    if (joinType != null && value.ToString().Contains("$"))//当是关联更新
                    {

                        if (!fields.ContainsKey(name))
                        {
                            throw new CRLException("找不到对应的字段,在" + typeof(T) + ",名称" + name);
                        }
                        var field = fields[name];
                        name = field.MapingName;//转换映射名

                        var fields2 = TypeCache.GetProperties(joinType, true);
                        var value1 = System.Text.RegularExpressions.Regex.Match(value.ToString(), @"\$(\w+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Groups[1].Value;
                        if (!fields2.ContainsKey(value1))
                        {
                            throw new CRLException("找不到对应的字段,在" + joinType + ",名称" + value1);
                        }
                        var field2 = fields2[value1];
                        value = value.ToString().Replace("$" + value1, "t2." + field2.MapingName);//右边字段需加前辍
                        name = string.Format("t1.{0}", name);
                    }
                    else
                    {
                        if (!fields.ContainsKey(name))
                        {
                            throw new CRLException("找不到对应的字段,在" + typeof(T) + ",名称" + name);
                        }
                        var field = fields[name];
                        name = field.MapingName;//转换映射名
                        string parame = string.Format("@{0}", name, dbContext.parIndex);
                        AddParam(name, value);
                        dbContext.parIndex += 1;
                        value = parame;
                    }
                    setString += string.Format(" {0}={1},", name, value);

                }
            }
            setString = setString.Substring(0, setString.Length - 1);
            return setString;
        }
        #region update


        /// <summary>
        /// 指定拼接条件更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="setValue"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        internal int Update<TModel>(ParameCollection setValue, string where) where TModel : IModel,new()
        {
            CheckTableCreated<TModel>();
            Type type = typeof(TModel);
            string table = TypeCache.GetTableName(type, dbContext);
            string setString = ForamtSetValue<TModel>(setValue);
            string sql = _DBAdapter.GetUpdateSql(table, setString, where);
            sql = _DBAdapter.SqlFormat(sql);
            int n = __DbHelper.Execute(sql);
            ClearParame();
            return n;
        }
        
        /// <summary>
        /// 按对象差异更新,由主键确定记录
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int Update<TModel>(TModel obj)
        {
            var c = GetUpdateField(obj);
            if (c.Count == 0)
            {
                return 0;
                //throw new CRLException("更新集合为空");
            }
            var primaryKey = TypeCache.GetTable(obj.GetType()).PrimaryKey;
            var keyValue = primaryKey.GetValue(obj);
            string where = string.Format("{0}=@{0}", primaryKey.MapingName);
            AddParam(primaryKey.MapingName, keyValue);
            int n = Update<TModel>(c, where);
            UpdateCacheItem(obj, c);
            if (n == 0)
            {
                throw new CRLException("更新失败,找不到主键为 " + keyValue + " 的记录");
            }
            obj.CleanChanges();
            return n;
        }

        /// <summary>
        /// 按完整查询条件进行更新
        /// goup语法不支持,其它支持
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        public override int Update<TModel>(LambdaQuery<TModel> query, ParameCollection updateValue)
        {
            var query1 = query as RelationLambdaQuery<TModel>;
            if (query1.__GroupFields.Count > 0)
            {
                throw new CRLException("update不支持group查询");
            }
            if (query1.__Relations.Count > 1)
            {
                throw new CRLException("update关联不支持多次");
            }
            if (updateValue.Count == 0)
            {
                throw new ArgumentNullException("更新时发生错误,参数值为空 ParameCollection setValue");
            }
            query1._IsRelationUpdate = true;
            var conditions = query1.GetQueryConditions(false).Trim();
            if (conditions.Length > 5)
            {
                conditions = conditions.Substring(5);
            }

            string table = query1.QueryTableName;
            table = query1.__DBAdapter.KeyWordFormat(table);
            query1.FillParames(this);
            //var properties = updateValue.GetType().GetProperties();

            if (query1.__Relations.Count > 0)
            {
                var kv = query1.__Relations.First();
                string setString = ForamtSetValue<TModel>(updateValue, kv.Key.OriginType);
                var t1 = query1.QueryTableName;
                var t2 = TypeCache.GetTableName(kv.Key.OriginType, query1.__DbContext);
                var join = query1.__Relations[kv.Key];
                join = join.Substring(join.IndexOf(" on ") + 3);
                if (!string.IsNullOrEmpty(conditions))
                {
                    join += " and ";
                }
                string sql = query1.__DBAdapter.GetRelationUpdateSql(t1, t2, join + conditions, setString);
                return Execute(sql);
            }
            else
            {
                conditions = conditions.Replace("t1.","");
            }
            return Update<TModel>(updateValue, conditions);
        }

        /// <summary>
        /// 关联更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TJoin"></typeparam>
        /// <param name="expression"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        public override int Update<TModel, TJoin>(Expression<Func<TModel, TJoin, bool>> expression, ParameCollection updateValue)
        {
            var query = new RelationLambdaQuery<TModel>(dbContext);
            query.Join<TJoin>(expression);
            return Update(query, updateValue);
        }

        #endregion
    }
}
